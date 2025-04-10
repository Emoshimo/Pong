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
    public Transform skillOptionsContainer;
    public Button continueButton;

    [Header("Skill Option Template")]
    public GameObject skillOptionPrefab;
    public SkillHoverDescription hoverDescriptionPrefab;

    // Current selection
    private List<GameObject> skillOptions = new List<GameObject>();
    private List<int> currentSkillIndices = new List<int>();
    private int skillsToSelect = 1;
    private int selectedCount = 0;
    private System.Action onSelectionComplete;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Hide panel by default
        selectionPanel.SetActive(false);
    }

    private void Start()
    {
        // Set up continue button
        continueButton.onClick.AddListener(FinishSelection);
        continueButton.interactable = false;
    }

    public void ShowSelectionUI(List<int> availableSkillIndices, int numToSelect, string title, 
                               string description, System.Action onComplete)
    {
        // Store data
        currentSkillIndices = availableSkillIndices;
        skillsToSelect = numToSelect;
        selectedCount = 0;
        onSelectionComplete = onComplete;
        
        // Update UI text
        
        // Clear existing options
        ClearSkillOptions();
        
        // Populate skill options
        foreach (int skillIndex in availableSkillIndices)
        {
            CreateSkillOption(skillIndex);
        }
        
        // Show panel and pause game
        selectionPanel.SetActive(true);
        Time.timeScale = 0;
        
        // Notify SkillManager we're in selection mode
        SkillManager.instance.ToggleSelectionMode(true);
        
        // Reset continue button
        continueButton.interactable = false;
    }

    private void CreateSkillOption(int skillIndex)
    {
        if (skillIndex >= SkillManager.instance.availableSkillPrefabs.Count)
            return;
            
        GameObject skillPrefab = SkillManager.instance.availableSkillPrefabs[skillIndex];
        Skill skillScript = skillPrefab.GetComponent<Skill>();
        
        // Instantiate option
        GameObject option = Instantiate(skillOptionPrefab, skillOptionsContainer);
        skillOptions.Add(option);
        
        // Set up option UI
        SkillOptionUI optionUI = option.GetComponent<SkillOptionUI>();
        if (optionUI != null)
        {
            optionUI.SetupOption(skillScript.skillName, skillScript.skillIcon, skillScript.GetSkillDescription());
            optionUI.skillIndex = skillIndex;
            optionUI.onSelected = OnSkillSelected;
        }
    }

    private void OnSkillSelected(int skillIndex, bool isSelected)
    {
        // Update selected count
        selectedCount += isSelected ? 1 : -1;
        
        // Enable continue button if enough skills are selected
        continueButton.interactable = (selectedCount >= skillsToSelect);
        
        // If multi-select is disabled and we only need one skill
        if (isSelected && skillsToSelect == 1 && selectedCount > 1)
        {
            // Deselect other options
            foreach (GameObject option in skillOptions)
            {
                SkillOptionUI optionUI = option.GetComponent<SkillOptionUI>();
                if (optionUI != null && optionUI.skillIndex != skillIndex && optionUI.isSelected)
                {
                    optionUI.SetSelected(false);
                    selectedCount--;
                }
            }
        }
    }

    private void FinishSelection()
    {
        // Collect selected skills
        List<int> selectedSkills = new List<int>();
        foreach (GameObject option in skillOptions)
        {
            SkillOptionUI optionUI = option.GetComponent<SkillOptionUI>();
            if (optionUI != null && optionUI.isSelected)
            {
                selectedSkills.Add(optionUI.skillIndex);
            }
        }
        
        // Apply selections to player (left paddle)
        ApplySelectedSkills(selectedSkills);
        
        // Hide panel and resume game
        selectionPanel.SetActive(false);
        Time.timeScale = 1;
        
        // Exit selection mode
        SkillManager.instance.ToggleSelectionMode(false);
        
        // Call completion callback
        onSelectionComplete?.Invoke();
    }

    private void ApplySelectedSkills(List<int> selectedSkills)
    {
        // Remove existing skills if needed
        if (SkillManager.instance.leftPaddleSkills.Count >= selectedSkills.Count)
        {
            // Remove skills from the end
            int removeCount = Mathf.Min(selectedSkills.Count, SkillManager.instance.leftPaddleSkills.Count);
            for (int i = 0; i < removeCount; i++)
            {
                SkillManager.instance.RemoveSkillFromPaddle(1, SkillManager.instance.leftPaddleSkills.Count - 1);
            }
        }
        
        // Add selected skills
        KeyCode[] keyCodes = new KeyCode[] { KeyCode.Q, KeyCode.E };
        for (int i = 0; i < selectedSkills.Count; i++)
        {
            // Use existing keys or default to Q, E if needed
            KeyCode keyToUse = KeyCode.Q;
            if (i < keyCodes.Length)
            {
                keyToUse = keyCodes[i];
            }
            
            SkillManager.instance.AssignSkillToPaddle(1, selectedSkills[i], keyToUse);
        }
    }

    private void ClearSkillOptions()
    {
        foreach (GameObject option in skillOptions)
        {
            Destroy(option);
        }
        skillOptions.Clear();
    }

    // Public method to close UI without selection (cancel)
    public void CancelSelection()
    {
        selectionPanel.SetActive(false);
        Time.timeScale = 1;
        SkillManager.instance.ToggleSelectionMode(false);
    }
}