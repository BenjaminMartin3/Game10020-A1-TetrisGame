using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class TetrisManager : MonoBehaviour
{
    public int score { get; private set; }
    public bool gameOver { get; private set; }

    // Variables for the timer 
    public float timerAmount { get; private set; }
    // Used so the timer can be rounded and isn't wonky with 8 decimal places 
    public float roundedTimer {  get; private set; }

    public UnityEvent OnScoreChanged;
    public UnityEvent OnTimerChanged;
    public UnityEvent OnGameOver;

    private void Start()
    {
        SetGameOver(false);
    }
    // Update Function is where the timer is run 
    private void Update()
    {
        // Amount of time on the timer subtracted bt delta time 
        timerAmount -= Time.deltaTime;
        // Rounds the timerAmount function to a whole number for the UI timer 
        roundedTimer = Mathf.Round(timerAmount);

        // Calls the SetGameOver function when timer runs out 
        if (timerAmount <= 0)
        {
            timerAmount = 0; 
            SetGameOver(true); 
        }

        // Unity Event to change the on screen timer 
        OnTimerChanged.Invoke();
    }

    public int CalculateScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        OnScoreChanged.Invoke();
    }

    public void SetGameOver(bool gameOver)
    {
        if (!gameOver)
        {
            score = 0;
            // Resets the timer back to 180 secs when game restarts 
            timerAmount = 180; 
            ChangeScore(0);
        }

        this.gameOver = gameOver;
        OnGameOver.Invoke();
    }
}
