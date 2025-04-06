using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleManager : MonoBehaviour
{
    public static PaddleManager instance;
    public Paddle leftPaddle;
    public Paddle rightPaddle;
    private void Awake() {
        if (!instance)
            instance = this;
        else {
            Destroy(instance);
        }

    }
    public void EnlargePaddle(float duration, float enlargementFactor, int paddle) 
    {
        StartCoroutine(EnlargePaddleForDuration(duration, enlargementFactor, paddle));
    }
    private IEnumerator EnlargePaddleForDuration(float duration, float enlargementFactor, int player)
    {
        Paddle paddle = player == 2 ? rightPaddle : leftPaddle;
        Vector3 originalScale = paddle.transform.localScale;
        paddle.transform.localScale = new Vector3(originalScale.x, originalScale.y * enlargementFactor, originalScale.z);
        yield return new WaitForSeconds(duration);
        paddle.transform.localScale = originalScale;
    }
    public void ShrinkPaddle(float duration, float shrinkFactor, int paddle)
    {
        StartCoroutine(ShrinkEnemyPaddleForDuration(duration,shrinkFactor, paddle));
    }
    private IEnumerator ShrinkEnemyPaddleForDuration(float duration, float shrinkFactor, int player)
    {
        Paddle paddle = player == 2 ? leftPaddle : rightPaddle;
        Vector3 originalScale = paddle.transform.localScale;
        paddle.transform.localScale = new Vector3(originalScale.x, originalScale.y * shrinkFactor, originalScale.z);
        yield return new WaitForSeconds(duration);
        paddle.transform.localScale = originalScale;
    }


}
