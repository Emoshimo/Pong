using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    
    [Header("Grid Configuration")]
    public int rows = 4;             // Horizontal rows
    public int columns = 5;          // Vertical columns
    public Vector2 playAreaSize = new Vector2(16, 10); // Size of play area in world units
    
    [Header("Debug Visualization")]
    public bool showGridDebug = true;
    public Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    
    // Cell tracking
    private GridCell[,] cells;
    private List<GridHazard> activeHazards = new List<GridHazard>();
    
    // Cell prefab for instantiation
    public GameObject cellPrefab;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeGrid();
    }
    
    private void InitializeGrid()
    {
        cells = new GridCell[rows, columns];
        
        // Calculate cell sizes
        float cellWidth = playAreaSize.x / rows;
        float cellHeight = playAreaSize.y / columns;
        
        // Calculate offset to center the grid
        float startX = -playAreaSize.x / 2 + cellWidth / 2;
        float startY = -playAreaSize.y / 2 + cellHeight / 2;
        
        // Create cells
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 position = new Vector3(
                    startX + r * cellWidth,
                    startY + c * cellHeight,
                    0
                );
                
                GameObject cellObj;
                if (cellPrefab != null)
                {
                    cellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                    cellObj.name = $"Cell_{r}_{c}";
                    
                    // Set cell size
                    cellObj.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                }
                else
                {
                    cellObj = new GameObject($"Cell_{r}_{c}");
                    cellObj.transform.position = position;
                    cellObj.transform.parent = transform;
                }
                
                // Add GridCell component
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.Initialize(r, c, new Vector2(cellWidth, cellHeight), position);
                cells[r, c] = cell;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGridDebug || cells == null) return;
        
        Gizmos.color = gridLineColor;
        
        // Draw grid cells
        foreach (GridCell cell in cells)
        {
            if (cell != null)
            {
                Vector3 pos = cell.position;
                Vector2 size = cell.size;
                
                // Draw rectangle
                Gizmos.DrawLine(new Vector3(pos.x - size.x/2, pos.y - size.y/2, 0), new Vector3(pos.x + size.x/2, pos.y - size.y/2, 0));
                Gizmos.DrawLine(new Vector3(pos.x + size.x/2, pos.y - size.y/2, 0), new Vector3(pos.x + size.x/2, pos.y + size.y/2, 0));
                Gizmos.DrawLine(new Vector3(pos.x + size.x/2, pos.y + size.y/2, 0), new Vector3(pos.x - size.x/2, pos.y + size.y/2, 0));
                Gizmos.DrawLine(new Vector3(pos.x - size.x/2, pos.y + size.y/2, 0), new Vector3(pos.x - size.x/2, pos.y - size.y/2, 0));
            }
        }
    }
    
    // Get the cell at a specific world position
    public GridCell GetCellAtPosition(Vector3 worldPosition)
    {
        // Convert world position to grid indices
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (cells[r, c].ContainsPosition(worldPosition))
                {
                    return cells[r, c];
                }
            }
        }
        
        return null;
    }
    
    // Get cell by grid indices
    public GridCell GetCell(int row, int column)
    {
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            return cells[row, column];
        }
        
        return null;
    }
    
    // Create a hazard at a specific cell
    public GridHazard CreateHazard(int row, int column, GameObject hazardPrefab, float duration = 5f)
    {
        GridCell cell = GetCell(row, column);
        if (cell == null || hazardPrefab == null)
            return null;
            
        // Create the hazard
        GameObject hazardObj = Instantiate(hazardPrefab, cell.position, Quaternion.identity);
        GridHazard hazard = hazardObj.GetComponent<GridHazard>();
        
        if (hazard != null)
        {
            hazard.Initialize(cell, duration);
            activeHazards.Add(hazard);
            cell.SetActiveHazard(hazard);
            return hazard;
        }
        
        Destroy(hazardObj);
        return null;
    }
    
    // Create a hazard in a random cell
    public GridHazard CreateRandomHazard(GameObject hazardPrefab, float duration = 5f)
    {
        int randomRow = Random.Range(0, rows);
        int randomColumn = Random.Range(0, columns);
        return CreateHazard(randomRow, randomColumn, hazardPrefab, duration);
    }
    
    // Create hazards in a row
    public List<GridHazard> CreateHazardRow(int rowIndex, GameObject hazardPrefab, float duration = 5f)
    {
        List<GridHazard> createdHazards = new List<GridHazard>();
        if (rowIndex < 0 || rowIndex >= rows) return createdHazards;
        
        for (int c = 0; c < columns; c++)
        {
            GridHazard hazard = CreateHazard(rowIndex, c, hazardPrefab, duration);
            if (hazard != null)
                createdHazards.Add(hazard);
        }
        
        return createdHazards;
    }
    
    // Create hazards in a column
    public List<GridHazard> CreateHazardColumn(int columnIndex, GameObject hazardPrefab, float duration = 5f)
    {
        List<GridHazard> createdHazards = new List<GridHazard>();
        if (columnIndex < 0 || columnIndex >= columns) return createdHazards;
        
        for (int r = 0; r < rows; r++)
        {
            GridHazard hazard = CreateHazard(r, columnIndex, hazardPrefab, duration);
            if (hazard != null)
                createdHazards.Add(hazard);
        }
        
        return createdHazards;
    }
    
    // Clear all hazards
    public void ClearAllHazards()
    {
        foreach (GridHazard hazard in activeHazards)
        {
            if (hazard != null)
            {
                hazard.Remove();
            }
        }
        
        activeHazards.Clear();
        
        // Reset all cells
        foreach (GridCell cell in cells)
        {
            cell.ClearHazard();
        }
    }
    
    // Clear hazards of a specific type
    public void ClearHazardsOfType<T>() where T : GridHazard
    {
        List<GridHazard> hazardsToRemove = new List<GridHazard>();
        
        foreach (GridHazard hazard in activeHazards)
        {
            if (hazard is T)
            {
                hazard.Remove();
                hazardsToRemove.Add(hazard);
                hazard.parentCell.ClearHazard();
            }
        }
        
        foreach (GridHazard hazard in hazardsToRemove)
        {
            activeHazards.Remove(hazard);
        }
    }
    
    // Check for hazards that have expired and remove them
    private void Update()
    {
        List<GridHazard> expiredHazards = new List<GridHazard>();
        
        foreach (GridHazard hazard in activeHazards)
        {
            if (hazard == null || hazard.IsExpired())
            {
                expiredHazards.Add(hazard);
            }
        }
        
        foreach (GridHazard hazard in expiredHazards)
        {
            if (hazard != null)
            {
                hazard.Remove();
            }
            activeHazards.Remove(hazard);
        }
    }
}