using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DataProtoAI : MonoBehaviour
{
    [Header("References")]
    public WayPointManager wpManager;
    public Transform[] waypoints;
    public Material[] materials;
    public int pointValue;
    public int health;

    NavMeshAgent myAgent;
    int waypointIndex;
    Vector3 targetLocation;


    // Start is called before the first frame update
    void Awake()
    {
        myAgent = GetComponent<NavMeshAgent>();
        wpManager = FindObjectOfType<WayPointManager>();
        waypoints = wpManager.waypoints;
        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, targetLocation) < 1)
        {
            Debug.Log("finding Target");
            IterateWaypointIndex();
            UpdateDestination();
        }
    }

    private void UpdateDestination()
    {
        targetLocation = waypoints[waypointIndex].position;
        Debug.Log("moving");
        myAgent.SetDestination(targetLocation);
    }

    private void IterateWaypointIndex()
    {
        
        if(waypointIndex == waypoints.Length)
        {
            Destroy(this);
        }
        else
        {
            waypointIndex++;
        }
    }

    public void LoseHealth()
    {
        health = health - 1;
        GetComponent<MeshRenderer>().material = materials[0];
        if(health >= 1)
        {
            StartCoroutine(RecoveryTime());
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator RecoveryTime()
    {
        yield return new WaitForSeconds(1);
    }
}
