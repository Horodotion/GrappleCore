using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public NIS_PlayerMovement pM;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(pM.playerID==0)
        {
            GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        }
        else 
        {
            GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
        }
    }

    private void Update()
    {
        float mouseX = pM.lookAxis.x * Time.deltaTime * sensX;
        float mouseY = pM.lookAxis.y * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}
