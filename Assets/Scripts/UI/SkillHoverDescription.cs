using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillHoverDescription : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public LayoutElement layoutElement;
    public CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    public float fadeInSpeed = 8f;
    public float fadeOutSpeed = 5f;
    public float preferredWidth = 300f;
    public int maxDescriptionLength = 150;
    
    private RectTransform rectTransform;
    private bool isVisible = false;
    private Vector2 targetPosition;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Add canvas group if not present
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        // Start invisible
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        
        // Create references if null
        if (layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();
    }
    
    public void Show(string title, string description, Vector2 position)
    {
        // Set content
        titleText.text = title;
        descriptionText.text = description;
        
        // Adjust width based on description length
        if (layoutElement != null)
        {
            bool useWideLayout = description.Length > maxDescriptionLength;
            layoutElement.preferredWidth = useWideLayout ? preferredWidth : 250;
        }
        
        // Position near the skill option
        targetPosition = position;
        rectTransform.position = position;
        
        // Make visible and start fade in
        gameObject.SetActive(true);
        isVisible = true;
        
        // Make sure we're on top of other UI elements
        transform.SetAsLastSibling();
        
        // Ensure it stays on screen
        ConstrainToScreen();
    }
    
    public void Hide()
    {
        isVisible = false;
    }
    
    private void Update()
    {
        // Handle fade in/out
        if (isVisible)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.unscaledDeltaTime * fadeInSpeed);
            rectTransform.position = Vector3.Lerp(rectTransform.position, targetPosition, Time.unscaledDeltaTime * 10f);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.unscaledDeltaTime * fadeOutSpeed);
            
            // Deactivate when fully transparent
            if (canvasGroup.alpha < 0.01f)
                gameObject.SetActive(false);
        }
    }
    
    // Method to ensure tooltip stays on screen
    public void ConstrainToScreen()
    {
        if (rectTransform == null) return;
        
        // Get the canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;
        
        // Get canvas rect
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // Get our size and position in screen space
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        // Convert to canvas space
        for (int i = 0; i < 4; i++)
            corners[i] = canvas.transform.InverseTransformPoint(corners[i]);
        
        // Calculate bounds
        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[1].y;
        
        // Calculate adjustments needed
        float adjustX = 0;
        float adjustY = 0;
        
        // Check if out of bounds (left/right)
        if (minX < -canvasRect.rect.width/2)
            adjustX = -canvasRect.rect.width/2 - minX;
        else if (maxX > canvasRect.rect.width/2)
            adjustX = canvasRect.rect.width/2 - maxX;
            
        // Check if out of bounds (top/bottom)
        if (minY < -canvasRect.rect.height/2)
            adjustY = -canvasRect.rect.height/2 - minY;
        else if (maxY > canvasRect.rect.height/2)
            adjustY = canvasRect.rect.height/2 - maxY;
            
        // Apply adjustment
        if (adjustX != 0 || adjustY != 0)
        {
            Vector3 adjustedPos = targetPosition;
            adjustedPos.x += adjustX;
            adjustedPos.y += adjustY;
            targetPosition = adjustedPos;
        }
    }
}