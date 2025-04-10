using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    
    // List of all available skill prefabs
    public List<GameObject> availableSkillPrefabs = new List<GameObject>();
    
    // List of currently equipped skills (max 2 per paddle)
    public List<Skill> leftPaddleSkills = new List<Skill>();
    public List<Skill> rightPaddleSkills = new List<Skill>();
    
    // Maximum number of skills per paddle
    public const int MAX_SKILLS_PER_PADDLE = 2;
    
    // References to skill prefabs
    public GameObject doubleBallSkillPrefab;
    public GameObject speedUpBallSkillPrefab;
    public GameObject slowDownBallSkillPrefab;
    public GameObject enlargePaddlePrefab;
    public GameObject shrinkPaddlePrefab;
    
    // Flag to indicate if the game is in skill selection mode
    private bool isInSelectionMode = false;
    
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
        
        // Add all skill prefabs to the available skills list
        availableSkillPrefabs.Add(doubleBallSkillPrefab);
        availableSkillPrefabs.Add(speedUpBallSkillPrefab);
        availableSkillPrefabs.Add(slowDownBallSkillPrefab);
        availableSkillPrefabs.Add(enlargePaddlePrefab);
        availableSkillPrefabs.Add(shrinkPaddlePrefab);
    }
    
    private void Start()
    {
        // Initially assign default skills as a fallback
        if (leftPaddleSkills.Count == 0 && rightPaddleSkills.Count == 0)
        {
            AssignDefaultSkills();
        }
    }
    
    void Update()
    {
        // Skip skill checking if in selection mode
        if (isInSelectionMode) return;
        
        // Activate skill based on input for left paddle
        foreach (var skill in leftPaddleSkills)
        {
            if (Input.GetKeyDown(skill.leftPaddleActivationKey) && !skill.IsOnCooldown(1))
            {
                skill.Activate(1);
            }
        }
        
        // Activate skill based on input for right paddle
        foreach (var skill in rightPaddleSkills)
        {
            if (Input.GetKeyDown(skill.rightPaddleActivationKey) && !skill.IsOnCooldown(2))
            {
                skill.Activate(2);
            }
        }
    }
    
    // Assign default skills at the start if no selection is made
    private void AssignDefaultSkills()
    {
        // Clear existing skills
        ClearAllSkills();
        
        // Assign first two skills to both paddles
        if (availableSkillPrefabs.Count >= 2)
        {
            AssignSkillToPaddle(1, 0, KeyCode.Q);  // Left paddle, first skill, Q key
            AssignSkillToPaddle(1, 1, KeyCode.E);  // Left paddle, second skill, E key
            AssignSkillToPaddle(2, 0, KeyCode.U);  // Right paddle, first skill, U key
            AssignSkillToPaddle(2, 1, KeyCode.O);  // Right paddle, second skill, O key
        }
    }
    
    // Clear all assigned skills
    public void ClearAllSkills()
    {
        // Destroy existing skill GameObjects
        foreach (var skill in leftPaddleSkills)
        {
            Destroy(skill.gameObject);
        }
        
        foreach (var skill in rightPaddleSkills)
        {
            Destroy(skill.gameObject);
        }
        
        leftPaddleSkills.Clear();
        rightPaddleSkills.Clear();
    }
    
    // Assign a specific skill to a paddle
    public void AssignSkillToPaddle(int paddleId, int skillIndex, KeyCode activationKey)
    {
        if (skillIndex < 0 || skillIndex >= availableSkillPrefabs.Count)
            return;
            
        Skill newSkill = Instantiate(availableSkillPrefabs[skillIndex]).GetComponent<Skill>();
        
        if (paddleId == 1)
        {
            if (leftPaddleSkills.Count >= MAX_SKILLS_PER_PADDLE)
            {
                Debug.LogWarning("Left paddle already has max skills. Remove one first.");
                Destroy(newSkill.gameObject);
                return;
            }
            
            newSkill.leftPaddleActivationKey = activationKey;
            leftPaddleSkills.Add(newSkill);
        }
        else if (paddleId == 2)
        {
            if (rightPaddleSkills.Count >= MAX_SKILLS_PER_PADDLE)
            {
                Debug.LogWarning("Right paddle already has max skills. Remove one first.");
                Destroy(newSkill.gameObject);
                return;
            }
            
            newSkill.rightPaddleActivationKey = activationKey;
            rightPaddleSkills.Add(newSkill);
        }
    }
    
    // Remove a skill from a paddle
    public void RemoveSkillFromPaddle(int paddleId, int skillIndex)
    {
        if (paddleId == 1 && skillIndex < leftPaddleSkills.Count)
        {
            Destroy(leftPaddleSkills[skillIndex].gameObject);
            leftPaddleSkills.RemoveAt(skillIndex);
        }
        else if (paddleId == 2 && skillIndex < rightPaddleSkills.Count)
        {
            Destroy(rightPaddleSkills[skillIndex].gameObject);
            rightPaddleSkills.RemoveAt(skillIndex);
        }
    }
    
    // Toggle skill selection mode
    public void ToggleSelectionMode(bool isActive)
    {
        isInSelectionMode = isActive;
    }
}