using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pM;
    public Transform cam;
    public Transform projectileSpawn;
    public LayerMask whatIsGrappleable;
    public LineRenderer myLR;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;


    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;


    private bool grappling;

    private void Start()
    {
        pM = GetComponent<PlayerMovement>();
        myLR.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(grappling)
        {
            myLR.SetPosition(0, projectileSpawn.position);
        }
    }

    private void StartGrapple()
    {
        if(grapplingCdTimer > 0 )
        {
            return;
        }

        grappling = true;
        pM.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        myLR.enabled = true;
        myLR.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pM.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePositionRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePositionRelativeYPos + overshootYAxis;

        if(grapplePositionRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        pM.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pM.freeze = false; 

        grappling = false;

        grapplingCdTimer = grapplingCd;

        myLR.enabled = false;
    }
}
