using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public ScoretText scoreTextPlayer1, scoretTextPlayer2;
    public GameObject menuObject;
    public Action onStartGame;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI volumeValueText;
    public TextMeshProUGUI playModeButtonText;
    public TextMeshProUGUI levelText;
    public GameObject pauseMenuPanel;
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;

    private void Start() {
        AdjustPlayModeText();
        GameManager.instance.onGameEnds += OnGameEnds;
    }
    public void UpdateScore(int scorePlayer1, int scorePlayer2)
    {
        scoretTextPlayer2.setScore(scorePlayer2);
        scoreTextPlayer1.setScore(scorePlayer1);
    }

    public void HighlightScore(int id) 
    {
        if (id == 1) 
        {
            scoreTextPlayer1.Highlight();
        }
        else {
            scoretTextPlayer2.Highlight();
        }
    }
    public void OnStartGameButtonEntered() 
    {
        menuObject.SetActive(false);
        onStartGame?.Invoke();
    }
    public void OnGameEnds(int winnerId)
    {
        menuObject.SetActive(true);
        winText.text = $"Player {winnerId} wins!";
    }
    private void OnDestroy() 
    {
        if (GameManager.instance)
        {
            GameManager.instance.onGameEnds -= OnGameEnds;
        }
    }
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        volumeValueText.text = $"{Mathf.RoundToInt(value*100)} %";
    }

    public void OnSwitchPlayModeButton()
    {
        GameManager.instance.SwitchPlayMode();
        AdjustPlayModeText();
    }

    private void AdjustPlayModeText()
    {
        switch (GameManager.instance.playMode)
        {
            case GameManager.PlayMode.PlayerVsPlayer:
                playModeButtonText.text = "2 Players";
                break;
            case GameManager.PlayMode.PlayerVsAi:
                playModeButtonText.text = "Player vs AI";
                break;
            case GameManager.PlayMode.AiVsAi:
                playModeButtonText.text = "Ai vs Ai";
                break;
        }
    }
    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }

    public void ShowPauseMenu(bool show)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(show);
        }
    }

    public void OnResumeButtonClicked()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.TogglePause();
        }
    }

    public void OnRestartLevelButtonClicked()
    {
        if (LevelManager.instance != null)
        {
            Time.timeScale = 1f; // Ensure time is running
            LevelManager.instance.RetryCurrentLevel();
            GameManager.instance.isPaused = false;
        }
    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f; // Ensure time is running
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ShowLevelComplete(int level, bool isFinalLevel)
    {
        if (levelCompletePanel != null)
        {
            // Configure level complete UI
            TextMeshProUGUI titleText = levelCompletePanel.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = $"Level {level} Complete!";
            }
            
            TextMeshProUGUI messageText = levelCompletePanel.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = isFinalLevel ? 
                    "Congratulations! You've completed all levels!" : 
                    "Great job! Ready for the next challenge?";
            }
            
            // Show the panel
            levelCompletePanel.SetActive(true);
        }
    }

    public void ShowGameOver(int level)
    {
        if (gameOverPanel != null)
        {
            // Configure game over UI
            TextMeshProUGUI titleText = gameOverPanel.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = "Game Over";
            }
            
            TextMeshProUGUI messageText = gameOverPanel.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = $"You were defeated on Level {level}. Would you like to try again?";
            }
            
            // Show the panel
            gameOverPanel.SetActive(true);
        }
    }
}
