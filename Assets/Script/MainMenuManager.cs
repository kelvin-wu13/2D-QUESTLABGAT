using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject highscorePanel;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI highscoreText;

    private void Start()
    {
        // Add null check for safety
        if (highscoreText == null)
        {
            Debug.LogWarning("Highscore Text is not assigned in MainMenuManager!");
            return;
        }

        ShowMainMenu();
        UpdateHighscoreDisplay();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ShowSettingsMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowHighscoreMenu()
    {
        mainMenuPanel.SetActive(false);
        highscorePanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        highscorePanel.SetActive(false);
    }

    private void UpdateHighscoreDisplay()
    {
        if (highscoreText != null)
        {
            float highscore = PlayerPrefs.GetFloat("Highscore", 0);
            highscoreText.text = "Highscore: " + Mathf.Floor(highscore).ToString();
        }
    }
}