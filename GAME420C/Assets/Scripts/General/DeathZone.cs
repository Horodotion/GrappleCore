using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public AudioManager audio;

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Hit");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PlayerHit");
            other.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage(100);
            audio.loseSound.Play();
        }
    }
}
