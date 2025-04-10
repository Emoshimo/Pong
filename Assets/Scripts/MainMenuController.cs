using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject levelSelectPanel;
    
    [Header("Level Select")]
    public TextMeshProUGUI currentLevelText;
    public Button continueButton;
    public Button newGameButton;
    
    
    private void Start()
    {
        // Show main panel by default
        ShowMainPanel();
        
        // Set up continue button
        if (continueButton != null)
        {
            bool hasSavedGame = PlayerPrefs.HasKey("CurrentLevel") && PlayerPrefs.GetInt("CurrentLevel", 1) > 1;
            continueButton.interactable = hasSavedGame;
            
            if (hasSavedGame && currentLevelText != null)
            {
                int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
                currentLevelText.text = $"Current Level: {savedLevel}";
            }
        }
        
        // Set up settings
        
    }
    
    
    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }
    
    public void ShowSettingsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }
    
    public void ShowCreditsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }
    
    public void ShowLevelSelectPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }
    
    
    
    public void OnStartNewGameClicked()
    {
        // Reset progress
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }
    
    public void OnContinueGameClicked()
    {
        // Load game scene (current level will be loaded by LevelManager)
        SceneManager.LoadScene("GameScene");
    }
    
    public void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void OnVolumeChanged(float value)
    {
        // Save volume setting
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        
        // Set audio listener volume
        AudioListener.volume = value;
    }
    
    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
}