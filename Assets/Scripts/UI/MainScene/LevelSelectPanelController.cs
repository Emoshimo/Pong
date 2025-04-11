using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectPanelController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentLevelText;
    public Button continueButton;
    public Button newGameButton;
    public Button backButton;
    
    private MainMenuController menuController;

    private void Start()
    {
        // Get reference to parent menu controller
        menuController = GetComponentInParent<MainMenuController>();
        
        // Set up button click events
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueGameClicked);
            
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnStartNewGameClicked);
            
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }
    
    private void OnEnable()
    {
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
            else if (currentLevelText != null)
            {
                currentLevelText.text = "No saved game";
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueGameClicked);
            
        if (newGameButton != null)
            newGameButton.onClick.RemoveListener(OnStartNewGameClicked);
            
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void OnContinueGameClicked()
    {
        // Load game scene (current level will be loaded by LevelManager)
        SceneManager.LoadScene("GameScene");
    }

    private void OnStartNewGameClicked()
    {
        // Reset progress
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }

    private void OnBackButtonClicked()
    {
        // Return to main panel
        if (menuController != null)
            menuController.ShowMainPanel();
    }
}