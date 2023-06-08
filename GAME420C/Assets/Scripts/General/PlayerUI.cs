using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI timerText;
    public GameObject winScreen;
    public TextMeshProUGUI winScreenText;

    [Header("Editables")]
    public int levelToLoad;
    public int thisLevel;

    public bool player1Win = false;
    public bool player2Win = false;
    public bool gameOver = false;

    public int score;
    public int timer;


    private void Start()
    {
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
        StartCoroutine(GameCounter());
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if(timer <= 0 && !gameOver)
        {
            gameOver = true;
            GameOver();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Application has been terminated");
    }

    public void LevelLoader()
    {
        SceneManager.LoadScene(levelToLoad);
        Debug.Log("loading");
    }

    public void Retry()
    {
        SceneManager.LoadScene(thisLevel);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameOverScreen.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        winScreen.SetActive(true);
        if (player1Win)
        {
            winScreenText.SetText("Player 1 Wins!");
        }
        else if(player2Win)
        {
            winScreenText.SetText("Player 2 Wins!");
        }
    }

    public void TeamWin()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        winScreen.SetActive(true);
        winScreenText.SetText("Total Score: " + score.ToString() + "!");
    }

    IEnumerator GameCounter()
    {
        WaitForSeconds wfs = new WaitForSeconds(1);
        while (true)
        {
            timerText.text = "Time Remaining: " + timer.ToString() + "s";
            yield return wfs;
            timer--;
        }
    }
}
   
