using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkPaddleSkill : Skill
{
    public float sizeMultiplier = 0.5f;
    public float duration = 5f;
    
    
    protected override void UseAbility(int paddleId)
    {
        // Find the opponent paddle (the one with a different ID)
        Paddle targetPaddle = null;
        
        // Get all paddles in the scene
        Paddle[] paddles = FindObjectsOfType<Paddle>();
        foreach (Paddle paddle in paddles)
        {
            if (paddle.id != paddleId)
            {
                targetPaddle = paddle;
                break;
            }
        }
        
        if (targetPaddle != null)
        {
            StartCoroutine(ShrinkPaddle(targetPaddle));
        }
        
        Debug.Log("Shrink Opponent skill activated by paddle " + paddleId);
    }
    
    private IEnumerator ShrinkPaddle(Paddle paddle)
    {
        Vector3 originalScale = paddle.transform.localScale;
        Vector3 newScale = originalScale;
        newScale.y *= sizeMultiplier;
        
        paddle.transform.localScale = newScale;
        
        yield return new WaitForSeconds(duration);
        
        paddle.transform.localScale = originalScale;
    }
}