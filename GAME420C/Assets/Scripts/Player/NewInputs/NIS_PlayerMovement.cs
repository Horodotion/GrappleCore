using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum MovementState
{
    walking,
    freeze,
    grounded,
    climbing,
    crouching,
    sliding,
    air,
    sprinting,
    grappling
}

public class NIS_PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float climbSpeed;
    public float slideSpeed;
    public float grappleStrength;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    public bool canJump = true;


    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    [HideInInspector] public InputAction onMove, onLook, onSprint, onCrouch, onFire1, onFire2, onSlide, onWeapon1, onWeapon2, onJump, onReload;
    [HideInInspector] public Vector2 moveAxis;
    [HideInInspector] public Vector2 lookAxis;
    public NIS_Climbing climbingScript;
    public NIS_Grappling grappleScript;
    public NIS_Sliding slidingScript;
    public NIS_GunSystem gunSystem;
    public PlayerHealth healthScript;
    public Transform orientation;
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public Vector3 moveDirection;
    public Rigidbody myRB;
    public Spawnpoints spawnPoints;
    [Header("UI References")]
    public Color defaultReticleColor;
    public Color grapplePointAvailableColor;
    public Image ourReticle;

    [Header("Multiplayer References")]
    public static NIS_PlayerMovement[] instance = new NIS_PlayerMovement[2];
    public int playerID;

    [Header("Camera Efects")]
    public PlayerCamera playerCam;
    public float grappleFOV = 80f;
    public float slideFOV = 80f;

    [Header("Movement State")]
    public MovementState state;
    public bool sliding;
    public bool crouching;
    public bool climbing;
    public bool freeze;
    private bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;



    private void Awake()
    {
        spawnPoints = FindObjectOfType<Spawnpoints>();
        AddNewPlayer(this);

        if (playerID == 0)
        {
            this.transform.position = spawnPoints.spawnPointsList[0].transform.position;
            this.transform.rotation = spawnPoints.spawnPointsList[0].transform.rotation;
            audioManager = FindObjectOfType<AudioManager>();
            audioManager.BGM.Play();
        }
        else
        {
            this.transform.position = spawnPoints.spawnPointsList[1].transform.position;
            this.transform.rotation = spawnPoints.spawnPointsList[1].transform.rotation;
        }
        
        input = gameObject.GetComponent<PlayerInput>();
        myRB = GetComponent<Rigidbody>();
        myRB.freezeRotation = true;

        canJump = true;
        startYScale = transform.localScale.y;

        onMove = input.currentActionMap.FindAction("Movement");
        onLook = input.currentActionMap.FindAction("Look");
        onSprint = input.currentActionMap.FindAction("Sprint");
        onCrouch = input.currentActionMap.FindAction("Crouch");
        onFire1 = input.currentActionMap.FindAction("Fire1");
        onFire2 = input.currentActionMap.FindAction("Fire2");
        onWeapon1 = input.currentActionMap.FindAction("Weapon1");
        onWeapon2 = input.currentActionMap.FindAction("Weapon2");
        onSlide = input.currentActionMap.FindAction("Slide");
        onJump = input.currentActionMap.FindAction("Jump");
        onReload = input.currentActionMap.FindAction("Reload");

        onJump.performed += OnJumpAction;
        onFire2.performed += OnGrappleAction;
        onReload.performed += OnReloadAction;
        onSlide.started += OnSlideAction;
        onFire1.performed += OnFire1Action;
        onWeapon1.performed += OnWeapon1Action;
        onWeapon2.performed += OnWeapon2Action;
    }


    private void Update()
    {
        moveAxis = onMove.ReadValue<Vector2>().normalized;
        lookAxis = onLook.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MyInput();
        SpeedControl();
        StateHandler();
        MovePlayer();


        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        //Handle Drag
        if (grounded && !grappleScript.grappling)
        {
            myRB.drag = groundDrag;
        }
        else if (OnSlope())
        {
            myRB.drag = groundDrag;
        }
        else
        {
            myRB.drag = 0;
        }

        if (myRB.velocity.magnitude < 100)
        {
            Debug.Log(state.ToString());
        }
        
    }

    private void MyInput()
    {
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        //verticalInput = Input.GetAxisRaw("Vertical");

        if (!grappleScript.grappling)
        {
            grappleScript.CheckForGrapplePoint();
        }
        else
        {
            if (onFire2.ReadValue<float>() <= 0.125 || Vector3.Distance(grappleScript.grapplePoint, myRB.worldCenterOfMass) > grappleScript.maxGrappleDistance)
            {
                grappleScript.StopGrapple();
            }
        }

        //Start Crouch
        if (onCrouch.ReadValue<float>() >= 0.125f)
        {
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            myRB.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else
        {
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            myRB.velocity = Vector3.zero;
        }
        //Mode - Climbing
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }
        //Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && myRB.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        //Mode - Crouching
        else if (onCrouch.ReadValue<float>() >= 0.125f)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        //Mode - Sprinting
        else if (grounded && onSprint.ReadValue<float>() >= 0.125f && !crouching)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        //Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else if (grappleScript.grappling)
        {
            state = MovementState.grappling;
        }
        //Mode - Air
        else
        {
            state = MovementState.air;
        }

        // Check if desiredMoveSpeed has changed a ton
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 6f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //Smoothly Lerp movespeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.fixedDeltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.fixedDeltaTime * speedIncreaseMultiplier;
            }

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    public void MovePlayer()
    {
        if (climbingScript.exitingWall)
        {
            return;
        }

        if (grappleScript.grappling)
        {
            grappleScript.GrappleUpdate();
            return;
        }

        moveDirection = orientation.forward * moveAxis.y + orientation.right * moveAxis.x;

        if (OnSlope() && !exitingSlope) //On Slope
        {
            myRB.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (myRB.velocity.y > 0)
            {
                myRB.AddForce(Vector3.down * 100f, ForceMode.Force);
            }
        }

        if (grounded) //On Ground
        {
            myRB.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded) //In Air
        {
            myRB.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //Turn off gravity while on slopes
        myRB.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (grappleScript.grappling)
        {
            return;
        }

        //Limit Speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (myRB.velocity.magnitude > moveSpeed)
            {
                myRB.velocity = myRB.velocity.normalized * moveSpeed;
            }
        }
        //Limit Speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);

            //Limit Velocity
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                myRB.velocity = new Vector3(limitedVel.x, myRB.velocity.y, limitedVel.z);
            }
        }
    }

    public void GetStunned()
    {
        StartCoroutine(Stunned());
    }

    private IEnumerator Stunned()
    {
        freeze = true;
        desiredMoveSpeed = crouchSpeed;
        yield return new WaitForSeconds(2);
        healthScript.Heal(100);
        freeze = false;
    }

    public void Jump()
    {
        exitingSlope = true;

        if (grappleScript.grappling)
        {
            grappleScript.StopGrapple();
        }

        if (climbingScript.wallFront && climbingScript.climbJumpsLeft > 0 && !grounded)
        {
            myRB.velocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);
            climbingScript.ClimbJump();
        }
        else
        {
            //Reset y velocity
            myRB.velocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);
            myRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        myRB.velocity = velocityToSet;

        playerCam.DoFOV(grappleFOV);
    }

    public void ResetRestrictions()
    {
        grappleScript.StopGrapple();
        playerCam.DoFOV(60f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<NIS_Grappling>().StopGrapple();
        }
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.4f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void ChangeReticleColor()
    {
        if (grappleScript.grapplePointAvailable)
        {
            ourReticle.color = grapplePointAvailableColor;
        }
        else
        {
            ourReticle.color = defaultReticleColor;
        }
    }

    // public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    // {
    //     float gravity = Physics.gravity.y;
    //     float displacementY = endPoint.y - startPoint.y;
    //     Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

    //     Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
    //     Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

    //     return velocityXZ + velocityY;
    // }


    public void AddNewPlayer(NIS_PlayerMovement playerToConnect)
    {

        for (int i = 0; i < NIS_PlayerMovement.instance.Length; i++)
        {
            if (NIS_PlayerMovement.instance[i] == null)
            {
                NIS_PlayerMovement.instance[i] = playerToConnect;
                playerToConnect.playerID = i;
                break;
            }
        }
    }

    public void OnJumpAction(InputAction.CallbackContext context)
    {
        // Debug.Log("Boing");
        if (canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    public void OnGrappleAction(InputAction.CallbackContext context)
    {
        // Debug.Log("Thwip");
        if (grappleScript.grapplePointAvailable)
        {
            grappleScript.StartGrapple();
        }
    }

    public void OnFire1Action(InputAction.CallbackContext context)
    {
        // Debug.Log("Pew pew");
        if (gunSystem.readyToShoot && !gunSystem.reloading && gunSystem.bulletsLeft > 0)
        {
            gunSystem.bulletsShot = gunSystem.bulletsPerTap;
            gunSystem.Shoot();
        }
    }

    public void OnReloadAction(InputAction.CallbackContext context)
    {
        // Debug.Log("Reload");
        if (gunSystem.bulletsLeft < gunSystem.magSize && !gunSystem.reloading)
        {
            gunSystem.Reload();
        }
    }

    public void OnWeapon1Action(InputAction.CallbackContext context)
    {
        // Debug.Log("Glock");
        if (!gunSystem.reloading)
        {
            gunSystem.EquipPistol();
            gunSystem.Reload();
        }
    }

    public void OnWeapon2Action(InputAction.CallbackContext context)
    {
        // Debug.Log("Shotty");
        if (!gunSystem.reloading)
        {
            gunSystem.EquipShotgun();
            gunSystem.Reload();
        }
    }

    public void OnSlideAction(InputAction.CallbackContext context)
    {
        // Debug.Log("Slide");
        //if (sliding)
        {
            slidingScript.SlidingMovement();
        }
    }
}
