using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        float currentScore = scoreManager.GetCurrentScore();
        currentScoreText.text = "Score: " + Mathf.Floor(currentScore).ToString();

        UpdateHighscore(currentScore);
    }

    private void UpdateHighscore(float currentScore)
    {
        float savedHighscore = PlayerPrefs.GetFloat("Highscore", 0);
        if (currentScore > savedHighscore)
        {
            PlayerPrefs.SetFloat("Highscore", currentScore);
            highscoreText.text = "New Highscore: " + Mathf.Floor(currentScore).ToString();
        }
        else
        {
            highscoreText.text = "Highscore: " + Mathf.Floor(savedHighscore).ToString();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}