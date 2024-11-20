using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float basePointsPerSecond = 1f;
    [SerializeField] private float pointsMultiplierIncreaseRate = 0.1f;
    [SerializeField] private float enemyKillPoints = 100f;
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI highScoreText;

    private float currentScore;
    private float pointsMultiplier = 1f;
    private float survivalTime;
    private bool isGameActive = true;
    private float highScore;

    private void Start()
    {
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        UpdateHighScoreDisplay();
        
        // Ensure score starts at 0
        currentScore = 0f;
    }

    private void Update()
    {
        if (!isGameActive) return;

        // Increase survival time
        survivalTime += Time.deltaTime;
        
        // Calculate dynamic points multiplier
        pointsMultiplier = 1f + (survivalTime * pointsMultiplierIncreaseRate);
        
        // Add points based on survival time
        currentScore += basePointsPerSecond * pointsMultiplier * Time.deltaTime;
        
        UpdateScoreDisplay();
        CheckAndUpdateHighScore();
    }

    public void AddEnemyKillPoints()
    {
        if (!isGameActive) return;

        // Add points for killing an enemy with multiplier
        currentScore += enemyKillPoints * pointsMultiplier;
    }

    private void CheckAndUpdateHighScore()
    {
        // Update high score if current score is higher
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
        }
    }

    public float GetCurrentScore()
    {
        return currentScore;
    }

    public void PauseScoring()
    {
        isGameActive = false;
    }

    public void ResumeScoring()
    {
        isGameActive = true;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.Floor(currentScore).ToString();
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + Mathf.Floor(highScore).ToString();
        }
    }

    // Reset score for game restart
    public void ResetScore()
    {
        currentScore = 0f;
        survivalTime = 0f;
        pointsMultiplier = 1f;
        isGameActive = true;
    }
}