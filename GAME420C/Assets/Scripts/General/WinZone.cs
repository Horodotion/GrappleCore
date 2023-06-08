using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    public AudioManager audio;
    public PlayerUI playerGUI;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponentInParent<NIS_PlayerMovement>().playerID == 0)
            {
                playerGUI.player1Win = true;
                playerGUI.Win();
            }
            else if(other.gameObject.GetComponentInParent<NIS_PlayerMovement>().playerID == 1)
            {
                playerGUI.player2Win = true;
                playerGUI.Win();
            }
        }
    }
}

