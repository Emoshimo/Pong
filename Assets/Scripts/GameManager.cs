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
        CheckWin();
    }

    private void CheckWin() 
    {
        int winnerID = scorePlayer1 == maxScore ? 1 : scorePlayer2 == maxScore ? 2 : 0;
        if (winnerID != 0)
        {
            //We have a winner
            onGameEnds.Invoke(winnerID);
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
    }
    private void OnDestroy()
    {
        gameUI.onStartGame -= OnStartGame;
        onGameEnds = null;
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
