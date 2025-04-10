using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Skill : MonoBehaviour
{
    public float cooldownTime = 5f;
    public string skillName = "Default Skill";
    public Sprite skillIcon;
    
    // Track cooldown for each paddle separately
    private bool isLeftPaddleOnCooldown = false;
    private bool isRightPaddleOnCooldown = false;
    
    public KeyCode leftPaddleActivationKey = KeyCode.None;
    public KeyCode rightPaddleActivationKey = KeyCode.None;

    public void Activate(int paddleId)
    {
        if (paddleId == 1 && !isLeftPaddleOnCooldown)
        {
            UseAbility(paddleId);
            StartCoroutine(CooldownCoroutine(paddleId));
        }
        else if (paddleId == 2 && !isRightPaddleOnCooldown)
        {
            UseAbility(paddleId);
            StartCoroutine(CooldownCoroutine(paddleId));
        }
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
    
    // Get the current cooldown status for a specific paddle
    public bool IsOnCooldown(int paddleId)
    {
        return paddleId == 1 ? isLeftPaddleOnCooldown : isRightPaddleOnCooldown;
    }
}