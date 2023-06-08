using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NIS_Grappling : MonoBehaviour
{
    [Header("References")]
    private NIS_PlayerMovement pM;
    public Transform cam;
    public Transform projectileSpawn;
    public LayerMask whatIsGrappleable;
    public LineRenderer myLR;

    [Header("Grappling")]
    [HideInInspector] public bool grapplePointAvailable;
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    
    public Vector3 grapplePoint;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;
    public bool grappling;
    

    private void Awake()
    {
        pM = GetComponent<NIS_PlayerMovement>();
        myLR.enabled = false;
    }

    private void FixedUpdate()
    {
        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            myLR.SetPosition(0, projectileSpawn.position);
        }
    }

    public void CheckForGrapplePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            if (!grapplePointAvailable)
            {
                grapplePointAvailable = true;
                pM.ChangeReticleColor();
            }
            grapplePoint = hit.point;
        }
        else if (grapplePointAvailable)
        {
            grapplePointAvailable = false;
            pM.ChangeReticleColor();
        }
    }

    public void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }

        grappling = true;

        if (grapplePointAvailable)
        {
            ExecuteGrapple();
        }
    }

    private void ExecuteGrapple()
    {
        pM.freeze = false;
        grappling = true;
        myLR.enabled = true;
        myLR.SetPosition(1, grapplePoint);

        Vector3 grappleDirection = (grapplePoint - pM.myRB.worldCenterOfMass).normalized;
        pM.myRB.AddForce(grappleDirection * pM.grappleStrength * 10f, ForceMode.Impulse);
    }

    public void GrappleUpdate()
    {
        Vector3 grappleDirection = (grapplePoint - pM.myRB.worldCenterOfMass).normalized;
        pM.myRB.AddForce(grappleDirection * pM.grappleStrength * 10f, ForceMode.Force);
    }

    public void StopGrapple()
    {
        pM.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        myLR.enabled = false;
    }
}
