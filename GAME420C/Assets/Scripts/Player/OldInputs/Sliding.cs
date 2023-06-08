using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody myRB;
    private PlayerMovement pM;


    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;


    [Header("Keybind and Inputs")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        myRB = GetComponent<Rigidbody>();
        pM = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (horizontalInput !=0 || verticalInput != 0))
        {
            StartSlide();
        }

        if(Input.GetKeyUp(slideKey) && pM.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if(pM.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        pM.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        myRB.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Sliding Normally
        if(!pM.OnSlope() || myRB.velocity.y > -0.1f)
        {
            pM.playerCam.DoFOV(pM.slideFOV);
            myRB.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        //Sliding on a slope
        else
        {
            if(myRB.velocity.y < 0)
            {
                pM.playerCam.DoFOV(pM.slideFOV);
                myRB.AddForce(pM.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
                myRB.AddForce(Vector3.down * 100f, ForceMode.Force);
            }   
        }


        if(slideTimer <= 0)
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
