using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float boost;
    
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("BouncyPad");
        collision.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * boost, ForceMode.Impulse);
    }
}
