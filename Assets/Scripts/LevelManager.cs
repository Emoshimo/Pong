using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    [Header("Level Configuration")]
    public int currentLevel = 1;
    public int maxLevel = 15;
    
    [Header("Level Win Conditions")]
    public int scoreToWin = 5;
    
    [Header("Level Data")]
    public List<LevelData> levelDataList = new List<LevelData>();
    
    [Header("UI References")]
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;
    
    private LevelData currentLevelData;
    private bool isLevelComplete = false;
    private bool gameOver = false;
    
    // Events
    public delegate void LevelEvent(int level);
    public event LevelEvent OnLevelStart;
    public event LevelEvent OnLevelComplete;
    public event LevelEvent OnGameOver;
    
    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedProgress();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Subscribe to game events
        if (GameManager.instance != null)
        {
            GameManager.instance.onScoreChanged += CheckWinCondition;
        }
        
        // Initialize the current level
        InitializeLevel(currentLevel);
    }
    
    public void InitializeLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        
        // Find level data
        currentLevelData = GetLevelData(currentLevel);
        
        // Apply level settings
        ApplyLevelSettings(currentLevelData);
        
        // Trigger level start event
        OnLevelStart?.Invoke(currentLevel);
        
        // Reset state
        isLevelComplete = false;
        gameOver = false;
        
        // Save current progress
        SaveProgress();
    }
    public LevelData GetCurrentLevelData()
    {
        // If currentLevelData is already loaded, return it directly
        if (currentLevelData != null)
        {
            return currentLevelData;
        }
        
        // Otherwise, get the level data for the current level
        return GetLevelData(currentLevel);
    }
    private LevelData GetLevelData(int level)
    {
        // Find level data in the list
        foreach (LevelData data in levelDataList)
        {
            if (data.levelNumber == level)
            {
                return data;
            }   
        }
        
        // Return default if not found
        Debug.LogWarning($"Level data for level {level} not found, using default settings");
        return CreateDefaultLevelData(level);
    }
    
    private LevelData CreateDefaultLevelData(int level)
    {
        LevelData defaultData = new LevelData();
        defaultData.levelNumber = level;
        defaultData.playerScoreToWin = 5;
        defaultData.aiDifficulty = 0.5f + (level * 0.05f); // Increases with level
        defaultData.hazardFrequency = Mathf.Min(0.1f + (level * 0.02f), 0.5f);
        
        // Basic default hazard types
        if (level >= 3) defaultData.availableHazards.Add(HazardType.Fire);
        if (level >= 5) defaultData.availableHazards.Add(HazardType.Water);
        if (level >= 8) defaultData.availableHazards.Add(HazardType.Air);
        
        return defaultData;
    }
    
    private void ApplyLevelSettings(LevelData data)
    {
        // Update win condition
        scoreToWin = data.playerScoreToWin;
        
        // Update AI difficulty for all AI-controlled paddles
        Paddle[] allPaddles = FindObjectsOfType<Paddle>();
        foreach (Paddle paddle in allPaddles)
        {
            if ((paddle.id == 1 && GameManager.instance.IsPlayer1Ai()) || 
                (paddle.id == 2 && GameManager.instance.IsPlayer2Ai()))
            {
                paddle.UpdateAIDifficulty(data.aiDifficulty, data.aiUsesSkills);
            }
        }
        
        // Set up hazards
        if (GridManager.instance != null)
        {
            SetupLevelHazards(data);
        }
        
        // Configure available skills for this level
        if (SkillManager.instance != null)
        {
            SetupLevelSkills(data);
        }
    }
    
    private void SetupLevelHazards(LevelData data)
    {
        // Clear existing hazards
        GridManager.instance.ClearAllHazards();
        
        // Setup hazard spawning based on level configuration
        StartCoroutine(SpawnHazardsRoutine(data));
    }
    
    private void SetupLevelSkills(LevelData data)
    {
        // Configure AI skills
        if (data.aiUsesSkills && data.aiSkills.Count > 0)
        {
            // Clear current AI skills
            SkillManager.instance.ClearRightPaddleSkills();
            
            // Assign skills to AI
            foreach (int skillIndex in data.aiSkills)
            {
                if (skillIndex < SkillManager.instance.availableSkillPrefabs.Count)
                {
                    SkillManager.instance.AssignSkillToPaddle(2, skillIndex, KeyCode.None);
                }
            }
        }
        
        // Set available skills for player to choose from
        // This will be used when showing skill selection UI
    }
    
    private IEnumerator SpawnHazardsRoutine(LevelData data)
    {
        while (!isLevelComplete && !gameOver)
        {
            // Wait for the next spawn
            float waitTime = 1f / data.hazardFrequency;
            yield return new WaitForSeconds(Random.Range(waitTime * 0.7f, waitTime * 1.3f));
            
            // Don't spawn if level complete or game over
            if (isLevelComplete || gameOver) break;
            
            // Spawn random hazard
            SpawnRandomHazard(data);
        }
    }
    
    private void SpawnRandomHazard(LevelData data)
    {
        if (data.availableHazards.Count == 0 || GridManager.instance == null)
            return;
            
        // Choose random hazard type
        HazardType hazardType = data.availableHazards[Random.Range(0, data.availableHazards.Count)];
        GameObject hazardPrefab = null;
        
        // Get the corresponding prefab
        switch (hazardType)
        {
            case HazardType.Fire:
                hazardPrefab = Resources.Load<GameObject>("Prefabs/Hazards/FireHazard");
                break;
            case HazardType.Water:
                hazardPrefab = Resources.Load<GameObject>("Prefabs/Hazards/WaterHazard");
                break;
            case HazardType.Air:
                hazardPrefab = Resources.Load<GameObject>("Prefabs/Hazards/AirHazard");
                break;
        }
        
        if (hazardPrefab != null)
        {
            // Determine placement strategy based on level
            if (Random.value < 0.2f && currentLevel > 4)
            {
                // 20% chance for row or column (from level 5+)
                if (Random.value < 0.5f)
                {
                    int row = Random.Range(0, GridManager.instance.rows);
                    GridManager.instance.CreateHazardRow(row, hazardPrefab, data.hazardDuration);
                }
                else
                {
                    int col = Random.Range(0, GridManager.instance.columns);
                    GridManager.instance.CreateHazardColumn(col, hazardPrefab, data.hazardDuration);
                }
            }
            else
            {
                // Regular single hazard
                GridManager.instance.CreateRandomHazard(hazardPrefab, data.hazardDuration);
            }
        }
    }
    
    private void CheckWinCondition(int player1Score, int player2Score)
    {
        // Check if player has won
        if (player1Score >= scoreToWin)
        {
            LevelComplete();
        }
        
        // Check if AI has won
        else if (player2Score >= scoreToWin)
        {
            GameOver();
        }
    }
    
    public void LevelComplete()
    {
        if (isLevelComplete) return;
        
        isLevelComplete = true;
        
        // Stop the game
        Time.timeScale = 0;
        
        // Show level complete screen
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        
        // Trigger event
        OnLevelComplete?.Invoke(currentLevel);
        
        // Auto-progress to next level if not at max
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            SaveProgress();
        }
    }
    
    public void GameOver()
    {
        if (gameOver) return;
        
        gameOver = true;
        
        // Stop the game
        Time.timeScale = 0;
        
        // Show game over screen
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Trigger event
        OnGameOver?.Invoke(currentLevel);
    }
    
    public void ContinueToNextLevel()
    {
        // Resume time
        Time.timeScale = 1;
        
        // Hide panels
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
            
        // Initialize next level
        InitializeLevel(currentLevel);
    }
    
    public void RetryCurrentLevel()
    {
        // Resume time
        Time.timeScale = 1;
        
        // Hide panels
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Restart current level
        InitializeLevel(currentLevel);
    }
    
    public void ReturnToMainMenu()
    {
        // Resume time
        Time.timeScale = 1;
        
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
    
    // Progress saving/loading
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }
    
    private void LoadSavedProgress()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }
        else
        {
            currentLevel = 1;
        }
    }
    
    public void ResetProgress()
    {
        currentLevel = 1;
        SaveProgress();
    }
}

// Class to hold data specific to each level
[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public string levelName;
    [TextArea] public string levelDescription;
    
    [Header("Win Conditions")]
    public int playerScoreToWin = 5;
    
    [Header("AI Settings")]
    public float aiDifficulty = 0.5f;
    public bool aiUsesSkills = false;
    public List<int> aiSkills = new List<int>(); // Indices of skills the AI will use
    
    [Header("Hazard Settings")]
    public float hazardFrequency = 0.1f;  // Hazards per second
    public float hazardDuration = 5f;
    public List<HazardType> availableHazards = new List<HazardType>();
    
    [Header("Player Skills")]
    public List<int> availableSkillsToChoose = new List<int>(); // For skill selection screen
}

public enum HazardType
{
    Fire,
    Water,
    Air
}