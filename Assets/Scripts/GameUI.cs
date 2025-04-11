using System;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public ScoretText scoreTextPlayer1, scoretTextPlayer2;
    public Action onStartGame;
    public TextMeshProUGUI volumeValueText;
    public TextMeshProUGUI playModeButtonText;
    public TextMeshProUGUI levelText;
    
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;

    private void Start()
    {
        // Initialize UI state
        AdjustPlayModeText();
        
        // Subscribe to event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnScoreUpdateRequested += UpdateScore;
            UIEventSystem.instance.OnScoreHighlightRequested += HighlightScore;
            UIEventSystem.instance.OnLevelTextUpdateRequested += UpdateLevelText;
            UIEventSystem.instance.OnVolumeChangeRequested += OnVolumeChanged;
            UIEventSystem.instance.OnPlayModeChangeRequested += OnPlayModeChanged;
            UIEventSystem.instance.OnPauseMenuRequested += ShowPauseMenuRequested;
            UIEventSystem.instance.OnResumeGameRequested += HidePauseMenuRequested;
            UIEventSystem.instance.OnGameOverDisplayRequested += ShowGameOverPanel;
            UIEventSystem.instance.OnLevelCompleteDisplayRequested += ShowLevelComplete;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnScoreUpdateRequested -= UpdateScore;
            UIEventSystem.instance.OnScoreHighlightRequested -= HighlightScore;
            UIEventSystem.instance.OnLevelTextUpdateRequested -= UpdateLevelText;
            UIEventSystem.instance.OnVolumeChangeRequested -= OnVolumeChanged;
            UIEventSystem.instance.OnPlayModeChangeRequested -= OnPlayModeChanged;
            UIEventSystem.instance.OnPauseMenuRequested -= ShowPauseMenuRequested;
            UIEventSystem.instance.OnResumeGameRequested -= HidePauseMenuRequested;
            UIEventSystem.instance.OnGameOverDisplayRequested -= ShowGameOverPanel;
            UIEventSystem.instance.OnLevelCompleteDisplayRequested -= ShowLevelComplete;
        }
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
        else
        {
            scoretTextPlayer2.Highlight();
        }
    }
    
    public void OnStartGameButtonEntered()
    {
        onStartGame?.Invoke();
    }

    private void ShowPauseMenuRequested()
    {
        pauseMenuPanel.SetActive(true);
    }

    private void HidePauseMenuRequested()
    {
        pauseMenuPanel.SetActive(false);
    }
    
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(value*100)} %";
        }
    }

    public void OnPlayModeButtonClicked()
    {
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestPlayModeChange();
        }
    }
    
    private void OnPlayModeChanged()
    {
        AdjustPlayModeText();
    }

    private void AdjustPlayModeText()
    {
        if (GameManager.instance == null || playModeButtonText == null)
            return;
            
        switch (GameManager.instance.playMode)
        {
            case GameManager.PlayMode.PlayerVsPlayer:
                playModeButtonText.text = "2 Players";
                break;
            case GameManager.PlayMode.PlayerVsAi:
                playModeButtonText.text = "Player vs AI";
                break;
            case GameManager.PlayMode.AiVsAi:
                playModeButtonText.text = "AI vs AI";
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

    public void ShowGameOverPanel(int level)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void ShowLevelComplete(int level, bool isFinalLevel)
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }
}