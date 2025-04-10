using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelTransitionUI : MonoBehaviour
{
    [Header("Level Complete")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteTitleText;
    public TextMeshProUGUI levelCompleteDescriptionText;
    public Button continueButton;
    public Button mainMenuButton;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverTitleText;
    public TextMeshProUGUI gameOverDescriptionText;
    public Button retryButton;
    public Button gameOverMainMenuButton;
    
    [Header("Skill Selection")]
    public SkillSelectionUI skillSelectionUI;
    
    private void Start()
    {
        // Hide panels by default
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
            
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Set up button events
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            
        if (gameOverMainMenuButton != null)
            gameOverMainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            
        // Register with LevelManager
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnLevelComplete += ShowLevelComplete;
            LevelManager.instance.OnGameOver += ShowGameOver;
            
            // Assign panels to LevelManager
            LevelManager.instance.levelCompletePanel = levelCompletePanel;
            LevelManager.instance.gameOverPanel = gameOverPanel;
        }
    }
    
    private void ShowLevelComplete(int level)
    {
        if (levelCompletePanel == null) return;
        
        levelCompletePanel.SetActive(true);
        
        // Set text
        if (levelCompleteTitleText != null)
            levelCompleteTitleText.text = $"Level {level} Complete!";
            
        if (levelCompleteDescriptionText != null)
        {
            if (level < LevelManager.instance.maxLevel)
                levelCompleteDescriptionText.text = "Congratulations! You've completed this level. Continue to the next level?";
            else
                levelCompleteDescriptionText.text = "Congratulations! You've completed all levels!";
        }
        
        // Check if we should show the skill selection UI
        if (level > 1 && level < LevelManager.instance.maxLevel && skillSelectionUI != null)
        {
            StartCoroutine(ShowSkillSelectionAfterDelay(1.5f));
        }
    }
    
    private IEnumerator ShowSkillSelectionAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // We use realtime since game is paused
        
        // Get level data to determine available skills
        LevelData currentLevelData = LevelManager.instance.levelDataList.Find(d => d.levelNumber == LevelManager.instance.currentLevel);
        
        if (currentLevelData != null && currentLevelData.availableSkillsToChoose.Count > 0)
        {
            // Hide level complete panel temporarily
            levelCompletePanel.SetActive(false);
            
            // Show skill selection
            skillSelectionUI.ShowSelectionUI(
                currentLevelData.availableSkillsToChoose, 
                1, 
                "Choose a Skill", 
                "Select a new skill to help you in the next level.",
                () => levelCompletePanel.SetActive(true) // Show level complete panel again after selection
            );
        }
    }
    
    private void ShowGameOver(int level)
    {
        if (gameOverPanel == null) return;
        
        gameOverPanel.SetActive(true);
        
        // Set text
        if (gameOverTitleText != null)
            gameOverTitleText.text = "Game Over";
            
        if (gameOverDescriptionText != null)
            gameOverDescriptionText.text = $"You were defeated on Level {level}. Would you like to try again?";
    }
    
    private void OnContinueButtonClicked()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.ContinueToNextLevel();
    }
    
    private void OnRetryButtonClicked()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.RetryCurrentLevel();
    }
    
    private void OnMainMenuButtonClicked()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.ReturnToMainMenu();
    }
    
    private void OnDestroy()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.OnLevelComplete -= ShowLevelComplete;
            LevelManager.instance.OnGameOver -= ShowGameOver;
        }
    }
}