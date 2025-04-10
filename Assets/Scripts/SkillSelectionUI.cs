using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelectionUI : MonoBehaviour
{
    public static SkillSelectionUI instance;
    
    [Header("UI References")]
    public GameObject selectionPanel;
    public Transform leftPaddleSkillContainer;
    public Transform rightPaddleSkillContainer;
    public Transform availableSkillsContainer;
    public Button startGameButton;
    
    [Header("Prefabs")]
    public GameObject skillButtonPrefab;
    
    [Header("Input")]
    public KeyCode openSelectionMenuKey = KeyCode.Tab;
    
    // References to UI elements
    private List<Button> leftPaddleButtons = new List<Button>();
    private List<Button> rightPaddleButtons = new List<Button>();
    private List<Button> availableSkillButtons = new List<Button>();
    
    // Currently selected skill for assignment
    private int selectedSkillIndex = -1;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Hide the selection panel initially
        selectionPanel.SetActive(false);
        
        // Set up the start game button
        startGameButton.onClick.AddListener(CloseSelectionPanel);
    }
    
    private void Update()
    {
        // Toggle skill selection menu when the key is pressed
        if (Input.GetKeyDown(openSelectionMenuKey))
        {
            ToggleSelectionPanel();
        }
    }
    
    private void ToggleSelectionPanel()
    {
        bool newState = !selectionPanel.activeSelf;
        selectionPanel.SetActive(newState);
        
        // Pause/unpause the game accordingly
        Time.timeScale = newState ? 0f : 1f;
        
        // Notify SkillManager about selection mode
        SkillManager.instance.ToggleSelectionMode(newState);
        
        // If opening the panel, refresh the UI
        if (newState)
        {
            RefreshUI();
        }
    }
    
    private void CloseSelectionPanel()
    {
        selectionPanel.SetActive(false);
        Time.timeScale = 1f;
        SkillManager.instance.ToggleSelectionMode(false);
    }
    
    private void RefreshUI()
    {
        // Clear existing UI elements
        ClearUIElements();
        
        // Populate the available skills
        PopulateAvailableSkills();
        
        // Populate the assigned skills for both paddles
        PopulateAssignedSkills();
    }
    
    private void ClearUIElements()
    {
        // Clear left paddle buttons
        foreach (var button in leftPaddleButtons)
        {
            Destroy(button.gameObject);
        }
        leftPaddleButtons.Clear();
        
        // Clear right paddle buttons
        foreach (var button in rightPaddleButtons)
        {
            Destroy(button.gameObject);
        }
        rightPaddleButtons.Clear();
        
        // Clear available skill buttons
        foreach (var button in availableSkillButtons)
        {
            Destroy(button.gameObject);
        }
        availableSkillButtons.Clear();
    }
    
    private void PopulateAvailableSkills()
    {
        // Create buttons for all available skills
        for (int i = 0; i < SkillManager.instance.availableSkillPrefabs.Count; i++)
        {
            int skillIndex = i; // Capture for lambda
            GameObject skillObj = SkillManager.instance.availableSkillPrefabs[i];
            Skill skill = skillObj.GetComponent<Skill>();
            
            GameObject buttonObj = Instantiate(skillButtonPrefab, availableSkillsContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set up button UI
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = skill.skillName;
            
            // If skill has an icon, set it
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (skill.skillIcon != null)
            {
                buttonImage.sprite = skill.skillIcon;
            }
            
            // Add click handler
            button.onClick.AddListener(() => {
                selectedSkillIndex = skillIndex;
                HighlightSelectedSkill(button);
            });
            
            availableSkillButtons.Add(button);
        }
    }
    
    private void PopulateAssignedSkills()
    {
        // Left paddle skills
        for (int i = 0; i < SkillManager.MAX_SKILLS_PER_PADDLE; i++)
        {
            GameObject buttonObj = Instantiate(skillButtonPrefab, leftPaddleSkillContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // Set to empty if slot isn't filled
            if (i < SkillManager.instance.leftPaddleSkills.Count)
            {
                Skill skill = SkillManager.instance.leftPaddleSkills[i];
                buttonText.text = skill.skillName + "\nKey: " + skill.leftPaddleActivationKey.ToString();
                
                // Set icon if available
                Image buttonImage = buttonObj.GetComponent<Image>();
                if (skill.skillIcon != null)
                {
                    buttonImage.sprite = skill.skillIcon;
                }
                
                // Add click handler to remove
                int skillIndex = i; // Capture for lambda
                button.onClick.AddListener(() => RemoveSkillFromPaddle(1, skillIndex));
            }
            else
            {
                buttonText.text = "Empty Slot\nClick to assign";
                
                // Add click handler to assign
                int slotIndex = i; // Capture for lambda
                button.onClick.AddListener(() => AssignSkillToPaddle(1, slotIndex));
            }
            
            leftPaddleButtons.Add(button);
        }
        
        // Right paddle skills
        for (int i = 0; i < SkillManager.MAX_SKILLS_PER_PADDLE; i++)
        {
            GameObject buttonObj = Instantiate(skillButtonPrefab, rightPaddleSkillContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // Set to empty if slot isn't filled
            if (i < SkillManager.instance.rightPaddleSkills.Count)
            {
                Skill skill = SkillManager.instance.rightPaddleSkills[i];
                buttonText.text = skill.skillName + "\nKey: " + skill.rightPaddleActivationKey.ToString();
                
                // Set icon if available
                Image buttonImage = buttonObj.GetComponent<Image>();
                if (skill.skillIcon != null)
                {
                    buttonImage.sprite = skill.skillIcon;
                }
                
                // Add click handler to remove
                int skillIndex = i; // Capture for lambda
                button.onClick.AddListener(() => RemoveSkillFromPaddle(2, skillIndex));
            }
            else
            {
                buttonText.text = "Empty Slot\nClick to assign";
                
                // Add click handler to assign
                int slotIndex = i; // Capture for lambda
                button.onClick.AddListener(() => AssignSkillToPaddle(2, slotIndex));
            }
            
            rightPaddleButtons.Add(button);
        }
    }
    
    private void HighlightSelectedSkill(Button selectedButton)
    {
        // Reset all button colors
        foreach (var button in availableSkillButtons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
        
        // Highlight the selected button
        selectedButton.GetComponent<Image>().color = Color.green;
    }
    
    private void AssignSkillToPaddle(int paddleId, int slotIndex)
    {
        // Make sure a skill is selected
        if (selectedSkillIndex == -1)
        {
            Debug.Log("Please select a skill first.");
            return;
        }
        
        KeyCode activationKey = KeyCode.None;
        
        // Determine the activation key based on the paddle and slot
        if (paddleId == 1)
        {
            activationKey = slotIndex == 0 ? KeyCode.Q : KeyCode.E;
        }
        else
        {
            activationKey = slotIndex == 0 ? KeyCode.U : KeyCode.O;
        }
        
        // First remove any existing skill in this slot
        if (paddleId == 1 && slotIndex < SkillManager.instance.leftPaddleSkills.Count)
        {
            SkillManager.instance.RemoveSkillFromPaddle(paddleId, slotIndex);
        }
        else if (paddleId == 2 && slotIndex < SkillManager.instance.rightPaddleSkills.Count)
        {
            SkillManager.instance.RemoveSkillFromPaddle(paddleId, slotIndex);
        }
        
        // Assign the selected skill
        SkillManager.instance.AssignSkillToPaddle(paddleId, selectedSkillIndex, activationKey);
        
        // Reset the selected skill
        selectedSkillIndex = -1;
        
        // Refresh UI
        RefreshUI();
    }
    
    private void RemoveSkillFromPaddle(int paddleId, int skillIndex)
    {
        SkillManager.instance.RemoveSkillFromPaddle(paddleId, skillIndex);
        RefreshUI();
    }
}