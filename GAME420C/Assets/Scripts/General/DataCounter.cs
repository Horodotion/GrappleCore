using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCounter : MonoBehaviour
{
    public int pointCounter;
    
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit A Thing");
        if (other.gameObject.CompareTag("DataSphere"))
        {
            pointCounter = pointCounter + other.gameObject.GetComponent<DataProtoAI>().pointValue;
            Destroy(other.gameObject);
            Debug.Log("Current Points: " + pointCounter.ToString());
        }
    }
}
