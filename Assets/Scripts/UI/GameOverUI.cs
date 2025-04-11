using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button retryButton;
    public Button mainMenuButton;

    private void Awake()
    {
        // Set up button listeners
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnEnable()
    {
        // Register with event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnGameOverDisplayRequested += SetupGameOver;
            UIEventSystem.instance.NotifyGameOverPanelVisibility(true);
        }
    }

    private void OnDisable()
    {
        // Unregister from event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnGameOverDisplayRequested -= SetupGameOver;
            UIEventSystem.instance.NotifyGameOverPanelVisibility(false);
        }
    }

    public void SetupGameOver(int level)
    {
        if (titleText != null)
        {
            titleText.text = "Game Over";
        }
        
        if (messageText != null)
        {
            messageText.text = $"You were defeated on Level {level}. Would you like to try again?";
        }
    }

    public void OnRetryClicked()
    {
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestRestartLevel();
        }
        gameObject.SetActive(false);
    }

    public void OnMainMenuClicked()
    {
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestMainMenu();
        }
    }
}