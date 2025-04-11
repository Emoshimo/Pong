using UnityEngine;
using UnityEngine.UI;

public class MainPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button exitButton;
    
    private MainMenuController menuController;

    private void Start()
    {
        // Get reference to parent menu controller
        menuController = GetComponentInParent<MainMenuController>();
        
        // Set up button click events
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            
        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
            
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        // Navigate to level select panel
        if (menuController != null)
            menuController.ShowLevelSelectPanel();
    }

    private void OnSettingsButtonClicked()
    {
        // Navigate to settings panel
        if (menuController != null)
            menuController.ShowSettingsPanel();
    }

    private void OnCreditsButtonClicked()
    {
        // Navigate to credits panel
        if (menuController != null)
            menuController.ShowCreditsPanel();
    }

    private void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}