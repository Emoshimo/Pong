using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnlargePaddleSkill : Skill
{
    public float sizeMultiplier = 1.5f;
    public float duration = 5f;

    protected override void UseAbility(int paddleId)
        {
            // Find the correct paddle
            Paddle targetPaddle = null;
            
            // Get all paddles in the scene
            Paddle[] paddles = FindObjectsOfType<Paddle>();
            foreach (Paddle paddle in paddles)
            {
                if (paddle.id == paddleId)
                {
                    targetPaddle = paddle;
                    break;
                }
            }
            
            if (targetPaddle != null)
            {
                StartCoroutine(EnlargePaddle(targetPaddle));
            }
            
            Debug.Log("Enlarge Paddle skill activated for paddle " + paddleId);
        }
    private IEnumerator EnlargePaddle(Paddle paddle)
    {
        Vector3 originalScale = paddle.transform.localScale;
        Vector3 newScale = originalScale;
        newScale.y *= sizeMultiplier;
        
        paddle.transform.localScale = newScale;
        
        yield return new WaitForSeconds(duration);
        
        paddle.transform.localScale = originalScale;
    }


}
