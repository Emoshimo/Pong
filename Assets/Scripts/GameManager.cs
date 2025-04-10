using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;  // makes GameManager Singleton
    public int scorePlayer1, scorePlayer2;
    public GameUI gameUI;
    public GameAudio gameAudio;
    public Shake screenShake;
    public Ball ball;
    public Action onReset;
    public Action<int> onGameEnds;
    public Action<int, int> onScoreChanged; // Add this event for score changes
    public int maxScore = 4;
    public PlayMode playMode; 

    public enum PlayMode {
        PlayerVsPlayer,
        PlayerVsAi,
        AiVsAi
    }

    private void Awake() {
        if (instance == null) 
        {
            instance = this;
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
    }

    private void OnLevelStart(int level)
    {
        // Reset game state for new level
        ResetGame();
        
        // Update UI to show current level
        if (gameUI != null)
        {
            gameUI.UpdateLevelText(level);
        }
    }

    public void ResetGame()
    {
        // Reset scores
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        
        // Update UI
        gameUI.UpdateScore(scorePlayer1, scorePlayer2);
        
        // Reset ball position and movement
        if (ball != null)
        {
            ball.ResetBall();
        }
        
        // Clear existing hazards if grid system is present
        if (GridManager.instance != null)
        {
            GridManager.instance.ClearAllHazards();
        }
        
        // Notify listeners that game has been reset
        onReset?.Invoke();
        
        // Reset any active skills
        if (SkillManager.instance != null)
        {
            SkillManager.instance.ClearAllSkills();
        }
        
        // Invoke score changed event with reset scores
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
        
        // Log for debugging
        Debug.Log("Game has been reset");
    }
    
    public void OnScoreZoneReached(int id) 
    {
        if (id == 1) {
            scorePlayer1++;
        }
        else {
            scorePlayer2++;
        }
        gameUI.UpdateScore(scorePlayer1, scorePlayer2);
        gameUI.HighlightScore(id);
        
        // Invoke the score changed event
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
        
        CheckWin();
    }

    private void CheckWin() 
    {
        int winnerID = scorePlayer1 == maxScore ? 1 : scorePlayer2 == maxScore ? 2 : 0;
        if (winnerID != 0)
        {
            //We have a winner
            onGameEnds?.Invoke(winnerID);
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
        gameUI.UpdateScore(scorePlayer1, scorePlayer2);
        
        // Invoke score changed event on game start
        onScoreChanged?.Invoke(scorePlayer1, scorePlayer2);
    }
    
    private void OnDestroy()
    {
        gameUI.onStartGame -= OnStartGame;
        onGameEnds = null;
        onScoreChanged = null;
    }

    public void SwitchPlayMode()
    {
        switch(playMode)
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
    }
    
    public bool IsPlayer2Ai()
    {
        return playMode == PlayMode.PlayerVsAi || playMode == PlayMode.AiVsAi;
    }
    public bool IsPlayer1Ai()
    {
        return playMode == PlayMode.AiVsAi;
    }

}
