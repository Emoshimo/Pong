using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridCellState
{
    Neutral,
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}

public class GridCell : MonoBehaviour
{
    // Cell properties
    public int row;
    public int column;
    public Vector2 size;
    public Vector3 position;
    
    // Current state
    public GridCellState state = GridCellState.Neutral;
    
    // Current active hazard
    public GridHazard activeHazard;
    
    // Visual elements
    private SpriteRenderer spriteRenderer;
    
    public void Initialize(int r, int c, Vector2 cellSize, Vector3 worldPosition)
    {
        row = r;
        column = c;
        size = cellSize;
        position = worldPosition;
        
        // Set up visuals if needed
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Check if this cell contains a world position
    public bool ContainsPosition(Vector3 worldPos)
    {
        float halfWidth = size.x / 2;
        float halfHeight = size.y / 2;
        
        return (worldPos.x >= position.x - halfWidth && worldPos.x <= position.x + halfWidth &&
                worldPos.y >= position.y - halfHeight && worldPos.y <= position.y + halfHeight);
    }
    
    // Set the active hazard for this cell
    public void SetActiveHazard(GridHazard hazard)
    {
        // Clear existing hazard
        if (activeHazard != null && activeHazard != hazard)
        {
            activeHazard.Remove();
        }
        
        activeHazard = hazard;
        
        // Update state based on hazard type
        UpdateState();
    }
    
    // Clear the current hazard
    public void ClearHazard()
    {
        activeHazard = null;
        state = GridCellState.Neutral;
        UpdateVisual();
    }
    
    // Update the cell state based on the active hazard
    private void UpdateState()
    {
        if (activeHazard == null)
        {
            state = GridCellState.Neutral;
        }
        else
        {
            // Determine state based on hazard type
            if (activeHazard is FireHazard) state = GridCellState.Fire;
            else if (activeHazard is WaterHazard) state = GridCellState.Water;
            else if (activeHazard is AirHazard) state = GridCellState.Air;
        }
        
        UpdateVisual();
    }
    
    // Update the visual appearance based on state
    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;
        
        // Set color based on state
        switch (state)
        {
            case GridCellState.Neutral:
                spriteRenderer.color = new Color(1, 1, 1, 0.1f);
                break;
            case GridCellState.Fire:
                spriteRenderer.color = new Color(1, 0.3f, 0, 0.4f);
                break;
            case GridCellState.Water:
                spriteRenderer.color = new Color(0, 0.5f, 1, 0.4f);
                break;
            case GridCellState.Earth:
                spriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 0.4f);
                break;
            case GridCellState.Air:
                spriteRenderer.color = new Color(0.8f, 0.8f, 1, 0.4f);
                break;
            case GridCellState.Light:
                spriteRenderer.color = new Color(1, 1, 0.8f, 0.4f);
                break;
            case GridCellState.Dark:
                spriteRenderer.color = new Color(0.3f, 0, 0.3f, 0.4f);
                break;
        }
    }
}