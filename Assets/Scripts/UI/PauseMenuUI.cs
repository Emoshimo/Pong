using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    private void Awake()
    {
        // Set up button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
        // Set up volume slider
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            UpdateVolumeText(AudioListener.volume);
        }
    }

    public void OnResumeClicked()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.TogglePause();
        }
    }

    public void OnRestartClicked()
    {
        if (LevelManager.instance != null)
        {
            Time.timeScale = 1f; // Ensure time is running
            LevelManager.instance.RetryCurrentLevel();
            GameManager.instance.isPaused = false;
        }
    }

    public void OnMainMenuClicked()
    {
        // Reset time scale and load main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);
    }
    
    private void UpdateVolumeText(float value)
    {
        if (volumeText != null)
        {
            volumeText.text = $"{Mathf.RoundToInt(value*100)} %";
        }
    }
}