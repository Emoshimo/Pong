using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelTestingTool : EditorWindow
{
    private int selectedLevel = 1;
    private Vector2 scrollPosition;
    private LevelManager levelManager;
    private List<string> levelNames = new List<string>();

    [MenuItem("Tools/Pong Level Testing")]
    public static void ShowWindow()
    {
        GetWindow<LevelTestingTool>("Level Testing");
    }

    private void OnEnable()
    {
        // Try to find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();
        RefreshLevelList();
    }

    private void RefreshLevelList()
    {
        levelNames.Clear();
        
        if (levelManager != null && levelManager.levelDataList != null)
        {
            foreach (LevelData levelData in levelManager.levelDataList)
            {
                string levelName = $"Level {levelData.levelNumber}: {levelData.levelName}";
                levelNames.Add(levelName);
            }
        }
        else
        {
            levelNames.Add("No levels found in LevelManager");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Pong Level Testing Tool", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Refresh Level List"))
        {
            levelManager = FindObjectOfType<LevelManager>();
            RefreshLevelList();
        }
        
        EditorGUILayout.Space();
        
        // Display current level from PlayerPrefs
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        EditorGUILayout.LabelField("Current Level (from PlayerPrefs)", currentLevel.ToString());
        
        EditorGUILayout.Space();
        
        // Level selection
        GUILayout.Label("Select Level to Test:", EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Show available levels
        if (levelNames.Count > 0)
        {
            for (int i = 0; i < levelNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(levelNames[i], GUILayout.Width(250)))
                {
                    selectedLevel = i + 1;
                }
                
                if (GUILayout.Button("Set", GUILayout.Width(50)))
                {
                    SetLevel(i + 1);
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        
        // Quick level set
        EditorGUILayout.BeginHorizontal();
        selectedLevel = EditorGUILayout.IntField("Level Number:", selectedLevel);
        
        if (GUILayout.Button("Set Level", GUILayout.Width(100)))
        {
            SetLevel(selectedLevel);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Reset progress button
        if (GUILayout.Button("Reset All Progress (Set to Level 1)"))
        {
            SetLevel(1);
            EditorUtility.DisplayDialog("Progress Reset", "Game progress has been reset to Level 1", "OK");
        }
        
        EditorGUILayout.Space();
        
        // Level details
        ShowLevelDetails();
    }
    
    private void SetLevel(int level)
    {
        // Ensure level is in valid range
        int maxLevel = levelManager != null ? levelManager.maxLevel : 15;
        level = Mathf.Clamp(level, 1, maxLevel);
        
        // Set the level in PlayerPrefs
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.Save();
        
        // Update the selected level in the editor
        selectedLevel = level;
        
        EditorUtility.DisplayDialog("Level Set", $"Current level set to {level}", "OK");
    }
    
    private void ShowLevelDetails()
    {
        if (levelManager == null || levelManager.levelDataList == null)
            return;
            
        // Find the level data for the selected level
        LevelData selectedLevelData = null;
        foreach (LevelData levelData in levelManager.levelDataList)
        {
            if (levelData.levelNumber == selectedLevel)
            {
                selectedLevelData = levelData;
                break;
            }
        }
        
        if (selectedLevelData != null)
        {
            GUILayout.Label("Level Details:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Name", selectedLevelData.levelName);
            EditorGUILayout.LabelField("Description", selectedLevelData.levelDescription);
            EditorGUILayout.LabelField("Win Score", selectedLevelData.playerScoreToWin.ToString());
            EditorGUILayout.LabelField("AI Difficulty", selectedLevelData.aiDifficulty.ToString());
            EditorGUILayout.LabelField("AI Uses Skills", selectedLevelData.aiUsesSkills.ToString());
            
            // Show hazards
            if (selectedLevelData.availableHazards.Count > 0)
            {
                string hazards = string.Join(", ", selectedLevelData.availableHazards);
                EditorGUILayout.LabelField("Hazards", hazards);
            }
            else
            {
                EditorGUILayout.LabelField("Hazards", "None");
            }
            
            // Show skills available for selection
            if (selectedLevelData.availableSkillsToChoose.Count > 0)
            {
                string skills = string.Join(", ", selectedLevelData.availableSkillsToChoose);
                EditorGUILayout.LabelField("Skill Choices", skills);
            }
            else
            {
                EditorGUILayout.LabelField("Skill Choices", "None");
            }
        }
        else
        {
            EditorGUILayout.HelpBox($"No level data found for Level {selectedLevel}", MessageType.Warning);
        }
    }
}