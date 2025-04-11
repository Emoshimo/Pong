using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int scorePlayer1, scorePlayer2;
    public GameUI gameUI;
    public GameAudio gameAudio;
    public Shake screenShake;
    public Ball ball;
    public Action onReset;
    public Action<int> onGameEnds;
    public Action<int, int> onScoreChanged;
    public int maxScore = 4;
    public PlayMode playMode;
    public bool isPaused = false;
    public Action<bool> onGamePaused;

    public enum PlayMode
    {
        PlayerVsPlayer,
        PlayerVsAi,
        AiVsAi
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (gameUI != null)
                gameUI.onStartGame += OnStartGame;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Connect with LevelManager
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnLevelStart += OnLevelStart;
        }
        
        // Connect with UI Event System
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnPauseMenuRequested += () => TogglePause();
            UIEventSystem.instance.OnResumeGameRequested += () => {
                if (isPaused) TogglePause();
            };
            UIEventSystem.instance.OnPlayModeChangeRequested += SwitchPlayMode;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    private void OnDestroy()
    {
        if (gameUI != null)
            gameUI.onStartGame -= OnStartGame;
            
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnLevelStart -= OnLevelStart;
        }
        
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnPauseMenuRequested -= () => TogglePause();
            UIEventSystem.instance.OnResumeGameRequested -= () => {
                if (isPaused) TogglePause();
            };
            UIEventSystem.instance.OnPlayModeChangeRequested -= SwitchPlayMode;
        }
        
        onReset = null;
        onGameEnds = null;
        onScoreChanged = null;
        onGamePaused = null;
    }
    
    private void OnLevelStart(int level)
    {
        ResetGame();
    }
    
    public void ResetGame()
    {
        // Reset scores
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        
        // Update UI using event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestScoreUpdate(scorePlayer1, scorePlayer2);
        }
        else if (gameUI != null)
        {
            // Fallback if event system is not available
            gameUI.UpdateScore(scorePlayer1, scorePlayer2);
        }
        
        // Reset ball
        if (ball != null)
        {
            ball.ResetBall();
        }
        
        // Clear existing hazards if grid system is present
        if (GridManager.instance != null)
        {
            GridManager.instance.ClearAllHazards();
        }
        
        // Reset any active skills
        if (SkillManager.instance != null)
        {
            SkillManager.instance.ClearAllSkills();
        }
        
        // Notify listeners that game has been reset
        onReset?.Invoke();
        
        // Invoke score changed event with reset scores
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
    }
    
    public void OnScoreZoneReached(int id)
    {
        if (id == 1)
        {
            scorePlayer1++;
        }
        else
        {
            scorePlayer2++;
        }
        
        // Update UI via event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestScoreUpdate(scorePlayer1, scorePlayer2);
            UIEventSystem.instance.RequestScoreHighlight(id);
        }
        else if (gameUI != null)
        {
            // Fallback if event system is not available
            gameUI.UpdateScore(scorePlayer1, scorePlayer2);
            gameUI.HighlightScore(id);
        }
        
        // Invoke the score changed event
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
        
        CheckWin();
    }
    
    private void CheckWin()
    {
        int winnerID = scorePlayer1 == maxScore ? 1 : scorePlayer2 == maxScore ? 2 : 0;
        if (winnerID != 0)
        {
            // We have a winner
            onGameEnds?.Invoke(winnerID);
            
            // Notify UI system of winner
            if (UIEventSystem.instance != null)
            {
                UIEventSystem.instance.RequestGameWinDisplay(winnerID);
            }
            
            gameAudio.PlayWinSound();
        }
        else
        {
            gameAudio.PlayScoreSound();
            onReset?.Invoke();
        }
    }
    
    private void OnStartGame()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        
        // Update UI using event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestScoreUpdate(scorePlayer1, scorePlayer2);
        }
        else if (gameUI != null)
        {
            gameUI.UpdateScore(scorePlayer1, scorePlayer2);
        }
        
        // Invoke score changed event on game start
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
    }

    public void SwitchPlayMode()
    {
        switch (playMode)
        {
            case PlayMode.PlayerVsPlayer:
                playMode = PlayMode.PlayerVsAi;
                break;
            case PlayMode.PlayerVsAi:
                playMode = PlayMode.AiVsAi;
                break;
            case PlayMode.AiVsAi:
                playMode = PlayMode.PlayerVsPlayer;
                break;
        }
        
        // Notify UI of play mode change
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestPlayModeChange();
        }
    }
    
    public bool IsPlayer2Ai()
    {
        return playMode == PlayMode.PlayerVsAi || playMode == PlayMode.AiVsAi;
    }
    
    public bool IsPlayer1Ai()
    {
        return playMode == PlayMode.AiVsAi;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        // Pause/unpause time
        Time.timeScale = isPaused ? 0f : 1f;
        
        // Show/hide pause menu using event system
        if (UIEventSystem.instance != null)
        {
            if (isPaused)
                UIEventSystem.instance.RequestPauseMenu();
            else
                UIEventSystem.instance.RequestResumeGame();
        }
        else if (gameUI != null)
        {
            // Fallback if event system is not available
            gameUI.ShowPauseMenu(isPaused);
        }
        
        // Notify any listeners
        onGamePaused?.Invoke(isPaused);
    }
}