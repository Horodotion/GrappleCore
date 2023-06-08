using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float climbSpeed;
    public float slideSpeed;

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
    


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;


    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Climbing climbingScript;
    public Transform orientation;


    [Header("Camera Efects")]
    public PlayerCamera playerCam;
    public float grappleFOV = 80f;
    public float slideFOV = 80f;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody myRB;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        climbing,
        crouching,
        sliding,
        air
    }

    public bool sliding;
    public bool crouching;
    public bool climbing;
    public bool freeze;

    public bool activeGrapple;

    private void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myRB.freezeRotation = true;

        canJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        
        MyInput();
        SpeedControl();
        StateHandler();

        //Handle Drag
        if(grounded && !activeGrapple)
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
    }

    private void FixedUpdate()
    {
        MovePlayer();

        //Debug.Log(state.ToString());
    }

    public void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //Start Crouch
        if(Input.GetKeyDown(crouchKey))
        {
            crouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            myRB.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //Stop Crouch
        if (Input.GetKeyUp(crouchKey))
        {
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //Mode - Climbing
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
        else if(sliding)
        {
            state = MovementState.sliding;

            if(OnSlope() && myRB.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        //Mode - Crouching
        else if(Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        //Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey) && !crouching)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        //Mode - Walking
        else if(grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        //Mode - Air
        else
        {
            state = MovementState.air;
        }

        // Check if desiredMoveSpeed has changed a ton
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 6f && moveSpeed != 0)
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

                time += Time.deltaTime *speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if(climbingScript.exitingWall || activeGrapple)
        {
            return;
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(OnSlope() && !exitingSlope) //On Slope
        {
            myRB.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(myRB.velocity.y > 0)
            {
                myRB.AddForce(Vector3.down * 100f, ForceMode.Force);
            }
        }

        if(grounded) //On Ground
        {
            myRB.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded) //In Air
        {
            myRB.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //Turn off gravity while on slopes
        myRB.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if(activeGrapple)
        {
            return;
        }
        
        //Limit Speed on slope
        if(OnSlope() && !exitingSlope)
        {
            if(myRB.velocity.magnitude > moveSpeed)
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

    public void Jump()
    {
        exitingSlope = true;
        
        //Reset y velocity
        myRB.velocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);

        myRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        myRB.velocity = velocityToSet;

        playerCam.DoFOV(grappleFOV);
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        playerCam.DoFOV(60f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f +0.4f))
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

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
