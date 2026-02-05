using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TetrisManager tetrisManager; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText; 
    public GameObject endgamePanel; 

    public void UIUpdateScore()
    {
        scoreText.text = $"SCORE: {tetrisManager.score}"; 
    }
    // Function to update the timer's UI 
    public void UIUpdateTimer()
    {
        timerText.text = $"TIME: {tetrisManager.roundedTimer}"; 
    }
    public void UpdateGameOver()
    {
        endgamePanel.SetActive(tetrisManager.gameOver);
    }
    public void PlayAgain()
    {
        tetrisManager.SetGameOver(false);
    }
}
