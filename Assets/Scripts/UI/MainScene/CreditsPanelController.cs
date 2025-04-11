using UnityEngine;
using UnityEngine.UI;

public class CreditsPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Button backButton;
    
    private MainMenuController menuController;

    private void Start()
    {
        // Get reference to parent menu controller
        menuController = GetComponentInParent<MainMenuController>();
        
        // Set up button click events
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        // Return to main panel
        if (menuController != null)
            menuController.ShowMainPanel();
    }
}