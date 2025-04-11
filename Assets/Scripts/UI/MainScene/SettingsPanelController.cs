using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValueText;
    public Toggle fullscreenToggle;
    public Button backButton;
    
    private MainMenuController menuController;
    private float defaultVolume = 0.8f;

    private void Start()
    {
        // Get reference to parent menu controller
        menuController = GetComponentInParent<MainMenuController>();
        
        // Set up button click events
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
            
        // Initialize volume slider
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        // Initialize fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
        
        // Update volume text if available
        UpdateVolumeText(volumeSlider.value);
    }

    private void OnEnable()
    {
        // Update controls with current values when panel becomes visible
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            UpdateVolumeText(volumeSlider.value);
        }
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackButtonClicked);
            
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggled);
    }

    public void OnVolumeChanged(float value)
    {
        // Save volume setting
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        
        // Set audio listener volume
        AudioListener.volume = value;
        
        // Update displayed value
        UpdateVolumeText(value);
    }
    
    private void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void OnBackButtonClicked()
    {
        // Return to main panel
        if (menuController != null)
            menuController.ShowMainPanel();
    }
}