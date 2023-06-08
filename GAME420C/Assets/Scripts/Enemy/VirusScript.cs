using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit A Thing");
        if (other.gameObject.CompareTag("DataSphere"))
        {
            Debug.Log("Hit DataSphere");
            other.gameObject.GetComponent<DataProtoAI>().LoseHealth();
        }
    }
}
