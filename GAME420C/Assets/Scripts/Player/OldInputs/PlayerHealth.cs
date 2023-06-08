using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public PlayerHealthBarBehavior healthBar;
    public NIS_PlayerMovement pM;

    [Header("Editables")]
    [SerializeField] private int maxHealth = 100;

    void Awake()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(health);
    }

    public void TakeDamage(int mod)
    {
        health -= mod;
        Debug.Log("Ow");
        //ouchSound.Play();
        healthBar.SetHealth(health);
        Debug.Log("ouch");

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            health = 0;
            Debug.Log("Stunned!");
            pM.GetStunned();
        }

    }

    public void Heal(int mod)
    {
        health += mod;
        healthBar.SetHealth(health);

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        else if (health <= 0)
        {
            health = 0;
            Debug.Log("You Died!");
            Time.timeScale = 0;
            //manager.GameOver();
        }

    }
}
