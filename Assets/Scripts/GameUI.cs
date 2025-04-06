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
}
