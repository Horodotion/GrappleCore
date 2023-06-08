using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody myRB;
    public PlayerMovement pM;
    public LayerMask whatIsWall;


    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;


    [Header("Climb Jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public int climbJumps;
    private int climbJumpsLeft;

    public KeyCode jumpKey = KeyCode.Space;


    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("ExitClimb")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private void Update()
    {
        WallCheck();
        StateMachine();
        
        if(climbing && !exitingWall)
        {
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        //State 1 - Climbing
        if(wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if(!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            //Start Timer
            if(climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }

            if(climbTimer <0)
            {
                StopClimbing();
            }
        }
        //State 2 - Exiting
        else if(exitingWall)
        {
            if(climbing)
            {
                StopClimbing();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer < 0)
            {
                exitingWall = false;
            }
        }
        //State 3 - None
        else
        {
            if(climbing)
            {
                StopClimbing();
            }
        }

        if(wallFront && Input.GetKeyDown(jumpKey) && climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if((wallFront && newWall) || pM.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        pM.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        myRB.velocity = new Vector3(myRB.velocity.x, climbSpeed, myRB.velocity.z);
    }

    private void StopClimbing()
    {
        climbing = false;
        pM.climbing = false;
    }

    private void ClimbJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        myRB.velocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);
        myRB.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
