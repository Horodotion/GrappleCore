using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NIS_Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody myRB;
    private NIS_PlayerMovement pM;
    public PlayerInput input;


    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideCooldown;
    public bool slideReady = true;


    public float slideYScale;
    private float startYScale;


    //[Header("Keybind and Inputs")]
    //public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        input = gameObject.GetComponent<PlayerInput>();
        myRB = GetComponent<Rigidbody>();
        pM = GetComponent<NIS_PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = pM.moveAxis.x;
        verticalInput = pM.moveAxis.y;

        if (pM.onSlide.ReadValue<float>() >= 0.125f && (horizontalInput != 0 || verticalInput != 0) && !pM.sliding && pM.grounded && slideReady && !pM.climbing)
        {
            StartSlide();
            StartCoroutine(SlideCooldownTimer());
        }

        if (pM.onSlide.ReadValue<float>() < 0.125f && pM.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pM.sliding)
        {
            SlidingMovement();
        }
    }

    private IEnumerator SlideCooldownTimer()
    {
        slideReady = false;
        yield return new WaitForSeconds(slideCooldown);
        slideReady = true;
    }

    private void StartSlide()
    {
        Debug.Log("Sliding Start");
        pM.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        myRB.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    public void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * pM.moveAxis.y + orientation.right * pM.moveAxis.x;


        //Sliding Normally
        if (!pM.OnSlope() || myRB.velocity.y > -0.1f)
        {
            pM.playerCam.DoFOV(pM.slideFOV);
            myRB.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        //Sliding on a slope
        else
        {
            if (myRB.velocity.y < 0)
            {
                pM.playerCam.DoFOV(pM.slideFOV);
                myRB.AddForce(pM.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
                myRB.AddForce(Vector3.down * 100f, ForceMode.Force);
            }
        }


        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        pM.sliding = false;
        pM.playerCam.DoFOV(60f);
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
