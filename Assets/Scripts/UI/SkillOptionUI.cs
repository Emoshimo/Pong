using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image skillIconImage;
    
    [Header("Hover Description")]
    public SkillHoverDescription hoverDescriptionPrefab;
    private SkillHoverDescription activeDescription;
    
    [Header("Hover Animation")]
    public float hoverScaleMultiplier = 1.05f;
    public float hoverAnimationSpeed = 10f;
    
    // Background button - the entire component is clickable
    private Button backgroundButton;
    private Vector3 originalScale;
    private bool isHovering = false;

    // Data
    public int skillIndex;
    public bool isSelected = false;
    public System.Action<int, bool> onSelected;
    
    // Store skill data
    private string skillTitle;
    private string skillDescription;

    private void Awake()
    {
        // Get the background button (should be on this GameObject)
        backgroundButton = GetComponent<Button>();
        if (backgroundButton == null)
        {
            Debug.LogError("SkillOptionUI must be attached to a GameObject with a Button component");
        }
        else
        {
            // Set up button to toggle selection
            backgroundButton.onClick.AddListener(ToggleSelected);
        }
        
        originalScale = transform.localScale;
    }

    private void Start()
    {
        UpdateVisualState();
    }
    
    private void Update()
    {
        // Smooth hover animation
        if (isHovering)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                originalScale * hoverScaleMultiplier, 
                Time.unscaledDeltaTime * hoverAnimationSpeed
            );
        }
        else
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                originalScale, 
                Time.unscaledDeltaTime * hoverAnimationSpeed
            );
        }
    }

    public void SetupOption(string skillName, Sprite icon, string description)
    {
        skillTitle = skillName;
        skillDescription = description;
        
        if (icon != null)
        {
            skillIconImage.sprite = icon;
            skillIconImage.gameObject.SetActive(true);
        }
        else
        {
            skillIconImage.gameObject.SetActive(false);
        }
        
        isSelected = false;
        UpdateVisualState();
    }
    
    // Implements IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        ShowDescription();
    }
    
    // Implements IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        HideDescription();
    }
    
    private void ShowDescription()
    {
        // Don't show duplicate descriptions
        if (activeDescription != null) return;
        
        // Get description prefab from SkillSelectionUI if not assigned
        if (hoverDescriptionPrefab == null && SkillSelectionUI.instance != null)
        {
            hoverDescriptionPrefab = SkillSelectionUI.instance.hoverDescriptionPrefab;
        }
        
        if (hoverDescriptionPrefab != null)
        {
            // Get the parent canvas to place our panel
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
            
            // Instantiate description panel as a sibling to this button
            // We want it as a child of the same parent that contains this skill option
            activeDescription = Instantiate(hoverDescriptionPrefab, transform.parent);
            
            // Calculate best position (above the button)
            Vector3 targetPos = transform.position;
            targetPos.y += GetComponent<RectTransform>().rect.height * 0.75f;
            
            // Show with content
            activeDescription.Show(skillTitle, skillDescription, targetPos);
        }
    }
    
    private void HideDescription()
    {
        if (activeDescription != null && !isSelected)
        {
            activeDescription.Hide();
            activeDescription = null;
        }
    }

    public void ToggleSelected()
    {
        SetSelected(!isSelected);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
        onSelected?.Invoke(skillIndex, isSelected);
        
        // Keep description visible if selected
        if (isSelected && activeDescription == null)
        {
            ShowDescription();
        }
        else if (!isSelected && !isHovering && activeDescription != null)
        {
            HideDescription();
        }
    }

    private void UpdateVisualState()
    {
        // Change the button's color to indicate selection state
        if (backgroundButton != null)
        {
            ColorBlock colors = backgroundButton.colors;
            if (isSelected)
            {
                // Make the selected state more vivid/prominent
                colors.normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            }
            else
            {
                // Default colors for unselected state
                colors.normalColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            }
            backgroundButton.colors = colors;
        }
    }
}