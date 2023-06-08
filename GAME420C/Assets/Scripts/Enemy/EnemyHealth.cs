using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    //public HealthBarBehavior healthBar;
    private bool deathCalled = false;
    private AudioManager audioManager;
    public GameObject toDestroy;

    //public GameObject deathEffect; (Death explosion?)


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        health = maxHealth;
        //healthBar.SetHealth(health, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        audioManager.ouchSound.Play();
        GetComponent<DamageFlash>().Flashing();

        if (health <= 0 && !deathCalled)
        {
            deathCalled = true;
            Die();
        }

    }

    void Die()
    {
        Debug.Log("I died");
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        audioManager.enemyDeathSound.Play();
        Destroy(toDestroy);
    }
}
