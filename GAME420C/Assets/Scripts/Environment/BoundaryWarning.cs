using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWarning : MonoBehaviour
{
    public GameObject playerGUI;
    public AudioManager audio;

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //playerGUI.GetComponent<PlayerUI>().BoundNear();
            audio.warningSound.Play();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //playerGUI.GetComponent<PlayerUI>().BoundFar();
            audio.warningSound.Pause();
        }
    }
}
