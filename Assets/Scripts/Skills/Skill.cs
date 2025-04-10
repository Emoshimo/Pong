using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

public abstract class Skill : MonoBehaviour
{
    [Header("Basic Skill Properties")]
    public string skillName = "Default Skill";
    public Sprite skillIcon;
    public SkillType skillType = SkillType.Active;
    public string skillDescription = "Skill description goes here.";
    
    [Header("Active Skill Properties")]
    public float cooldownTime = 5f;
    public float manaCost = 10f;
    
    // Track cooldown for each paddle separately
    private bool isLeftPaddleOnCooldown = false;
    private bool isRightPaddleOnCooldown = false;
    
    // Track effect duration
    protected float effectDuration = 0f;
    protected bool isEffectActive = false;
    
    public KeyCode leftPaddleActivationKey = KeyCode.None;
    public KeyCode rightPaddleActivationKey = KeyCode.None;

    protected virtual void Start()
    {
        // Initialize skill
        if (skillType == SkillType.Passive)
        {
            // Auto-activate passive skills
            ActivatePassive(1); // For left paddle
            ActivatePassive(2); // For right paddle
        }
    }
    
    protected virtual void Update()
    {
        // For skills that need continuous updates
    }
    
    public virtual string GetSkillDescription()
    {
        // Extend description with gameplay details
        string fullDescription = skillDescription;
        
        if (skillType == SkillType.Active)
        {
            fullDescription += $"\n\nCooldown: {cooldownTime} seconds";
            fullDescription += $"\nMana Cost: {manaCost}";
        }
        
        return fullDescription;
    }

    public void Activate(int paddleId)
    {
        if (skillType == SkillType.Passive) return; // Passive skills aren't manually activated
        
        // Check cooldown
        if (paddleId == 1 && !isLeftPaddleOnCooldown)
        {
            // Check if player has enough mana (would be added in a mana system)
            UseAbility(paddleId);
            StartCoroutine(CooldownCoroutine(paddleId));
        }
        else if (paddleId == 2 && !isRightPaddleOnCooldown)
        {
            UseAbility(paddleId);
            StartCoroutine(CooldownCoroutine(paddleId));
        }
    }

    // Method for passive skills
    protected virtual void ActivatePassive(int paddleId)
    {
        // Override in passive skill implementations
    }
    
    // Method for deactivating passive skills
    protected virtual void DeactivatePassive(int paddleId)
    {
        // Override in passive skill implementations
    }

    protected abstract void UseAbility(int paddleId);

    private IEnumerator CooldownCoroutine(int paddleId)
    {
        if (paddleId == 1)
            isLeftPaddleOnCooldown = true;
        else if (paddleId == 2)
            isRightPaddleOnCooldown = true;
            
        yield return new WaitForSeconds(cooldownTime);
        
        if (paddleId == 1)
            isLeftPaddleOnCooldown = false;
        else if (paddleId == 2)
            isRightPaddleOnCooldown = false;
    }
    
    // Start effect duration timer
    protected IEnumerator EffectDurationCoroutine(float duration, int paddleId)
    {
        isEffectActive = true;
        yield return new WaitForSeconds(duration);
        EndEffect(paddleId);
        isEffectActive = false;
    }
    
    // Called when effect duration ends
    protected virtual void EndEffect(int paddleId)
    {
        // Override in skill implementations to clean up effects
    }
    
    // Get the current cooldown status for a specific paddle
    public bool IsOnCooldown(int paddleId)
    {
        return paddleId == 1 ? isLeftPaddleOnCooldown : isRightPaddleOnCooldown;
    }
    
    // For UI to show cooldown progress
    public float GetCooldownProgress(int paddleId)
    {
        // Add implementation to track and return cooldown progress (0-1)
        // Would need coroutine modification to track elapsed time
        return 0f;
    }
    
    // Called when the skill is removed or gameplay ends
    protected virtual void OnDisable()
    {
        // Clean up any active effects
        if (isEffectActive)
        {
            EndEffect(1);
            EndEffect(2);
        }
        
        // Clean up passive effects
        if (skillType == SkillType.Passive)
        {
            DeactivatePassive(1);
            DeactivatePassive(2);
        }
    }
}