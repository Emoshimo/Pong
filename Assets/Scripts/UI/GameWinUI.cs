using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameWinUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button nextLevelButton;
    public Button mainMenuButton;
    public Button restartButton;

    private void Awake()
    {
        // Set up button listeners
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void OnEnable()
    {
        // Register with event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnGameWinDisplayRequested += ShowWinner;
            UIEventSystem.instance.OnLevelCompleteDisplayRequested += SetupLevelComplete;
            UIEventSystem.instance.NotifyGameWinPanelVisibility(true);
        }
    }

    private void OnDisable()
    {
        // Unregister from event system
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.OnGameWinDisplayRequested -= ShowWinner;
            UIEventSystem.instance.OnLevelCompleteDisplayRequested -= SetupLevelComplete;
            UIEventSystem.instance.NotifyGameWinPanelVisibility(false);
        }
    }

    public void SetupLevelComplete(int level, bool isFinalLevel)
    {
        if (titleText != null)
        {
            titleText.text = $"Level {level} Complete!";
        }
        
        if (messageText != null)
        {
            messageText.text = isFinalLevel ? 
                "Congratulations! You've completed all levels!" : 
                "Great job! Ready for the next challenge?";
        }
        
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(!isFinalLevel);
        }
    }
    
    public void ShowWinner(int winnerId)
    {
        if (titleText != null)
        {
            titleText.text = $"Player {winnerId} Wins!";
        }
    }

    public void OnNextLevelClicked()
    {
        if (UIEventSystem.instance != null)
        {
            UIEventSystem.instance.RequestNextLevel();
        }
        gameObject.SetActive(false);
    }
    
    public void OnRestartClicked()
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