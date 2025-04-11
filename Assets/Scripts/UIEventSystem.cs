using System;
using UnityEngine;

public class UIEventSystem : MonoBehaviour
{
    public static UIEventSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // UI Panel Events
    public event Action OnPauseMenuRequested;
    public event Action OnResumeGameRequested;
    public event Action<int> OnGameWinDisplayRequested;
    public event Action<int, bool> OnLevelCompleteDisplayRequested;
    public event Action<int> OnGameOverDisplayRequested;
    public event Action OnMainMenuRequested;
    public event Action OnRestartLevelRequested;
    public event Action OnNextLevelRequested;
    
    // Game UI Events
    public event Action<int, int> OnScoreUpdateRequested;
    public event Action<int> OnScoreHighlightRequested;
    public event Action<int> OnLevelTextUpdateRequested;
    public event Action<float> OnVolumeChangeRequested;
    public event Action OnPlayModeChangeRequested;
    
    // Panel Visibility Events
    public event Action<bool> OnPauseMenuVisibilityChanged;
    public event Action<bool> OnGameWinPanelVisibilityChanged;
    public event Action<bool> OnGameOverPanelVisibilityChanged;

    // Methods to trigger events
    public void RequestPauseMenu() => OnPauseMenuRequested?.Invoke();
    public void RequestResumeGame() => OnResumeGameRequested?.Invoke();
    public void RequestGameWinDisplay(int winnerId) => OnGameWinDisplayRequested?.Invoke(winnerId);
    public void RequestLevelCompleteDisplay(int level, bool isFinalLevel) => OnLevelCompleteDisplayRequested?.Invoke(level, isFinalLevel);
    public void RequestGameOverDisplay(int level) => OnGameOverDisplayRequested?.Invoke(level);
    public void RequestMainMenu() => OnMainMenuRequested?.Invoke();
    public void RequestRestartLevel() => OnRestartLevelRequested?.Invoke();
    public void RequestNextLevel() => OnNextLevelRequested?.Invoke();
    
    public void RequestScoreUpdate(int player1Score, int player2Score) => OnScoreUpdateRequested?.Invoke(player1Score, player2Score);
    public void RequestScoreHighlight(int playerId) => OnScoreHighlightRequested?.Invoke(playerId);
    public void RequestLevelTextUpdate(int level) => OnLevelTextUpdateRequested?.Invoke(level);
    public void RequestVolumeChange(float volume) => OnVolumeChangeRequested?.Invoke(volume);
    public void RequestPlayModeChange() => OnPlayModeChangeRequested?.Invoke();
    
    public void NotifyPauseMenuVisibility(bool isVisible) => OnPauseMenuVisibilityChanged?.Invoke(isVisible);
    public void NotifyGameWinPanelVisibility(bool isVisible) => OnGameWinPanelVisibilityChanged?.Invoke(isVisible);
    public void NotifyGameOverPanelVisibility(bool isVisible) => OnGameOverPanelVisibilityChanged?.Invoke(isVisible);
}