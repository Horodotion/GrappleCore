using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZonePvE : MonoBehaviour
{
    public AudioManager audio;
    public PlayerUI playerGUI;
    public DataCounter dataCounter;
    public GameObject[] livingDataSpheres;
    public int score;
    public bool player1Goal = false;
    public bool player2Goal = false;
    public bool messageSent = false;
    public GameObject[] playersConnected;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponentInParent<NIS_PlayerMovement>().playerID == 0)
            {
                player1Goal = true;
            }
            else if (other.gameObject.GetComponentInParent<NIS_PlayerMovement>().playerID == 1)
            {
                player2Goal = true;
            }
        }
    }

    private void Update()
    {
        playersConnected = GameObject.FindGameObjectsWithTag("Player");

        if (playersConnected.Length <= 5)
        {
            if (player1Goal && !messageSent)
            {
                messageSent = true;
                TeamWin();
            }
        }
        if (playersConnected.Length > 5)
        {
            if (player1Goal && player2Goal && !messageSent)
            {
                messageSent = true;
                TeamWin();
            }
        }
    }

    private void TeamWin()
    {
        livingDataSpheres = GameObject.FindGameObjectsWithTag("DataSphere");
        score = score + (livingDataSpheres.Length * 100);
        score = score + dataCounter.pointCounter;
        score = score + (playerGUI.timer * 100);
        playerGUI.score = score;
        //playerGUI.score;
        playerGUI.TeamWin();
    }
}
