using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject projectile;
    public Transform shootPoint;


    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;


    [Header("Attacking")]
    public float attackCooldown;
    bool hasAttacked;

    [Header("State Checks")]
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        //player = GameObject.Find("PlayerController").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRange, -1);

        foreach (var col in hitColliders)
        {
            if (col.gameObject.tag == "Player" && col.GetComponentInParent<PlayerController>() != null)
            {
                Debug.Log(col.gameObject.name);
                player = col.gameObject.transform;
                break;
            }
        }

        if(player != null)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, - 1);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, -1);
        }


        if(!playerInSightRange && !playerInAttackRange)
        {
            Patrol();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            Attack();
        }
    }

    private void Patrol()
    {
        //Check for Walkpoint
        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        //Move to walkpoint
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Check for new walkpoint
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        //Stop Enemy and look at player
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!hasAttacked)
        {
            //Attack
            Rigidbody rB = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rB.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rB.AddForce(transform.up * 8f, ForceMode.Impulse);

            hasAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }
}
