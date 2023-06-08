using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("hit");
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("player hit");
            collision.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("pass");
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionHit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Destroy(this.gameObject);
        }
    }
}
