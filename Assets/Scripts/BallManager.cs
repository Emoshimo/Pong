using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;
    public Ball originalBall;
    private List<Ball> balls = new List<Ball>();

    private void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    private void Start() 
    {
        GameManager.instance.onReset += ScoreAction;
        GameManager.instance.onGameEnds += GameOverAction;
    }
    public void DuplicateBall(float duration)
    {
        StartCoroutine(DuplicateBallForDuration(duration));
    }
    private IEnumerator DuplicateBallForDuration(float duration)
    {
        // Create a new GameObject for the duplicated ball
        Ball duplicatedBall = Instantiate(originalBall, originalBall.transform.position, originalBall.transform.rotation);
        duplicatedBall.IsClone = true;
        Rigidbody2D rb = duplicatedBall.GetComponent<Rigidbody2D>();
        rb.velocity = originalBall.GetComponent<Rigidbody2D>().velocity;

        // Angle Change
        float angleInDegrees = 30f;
        rb.velocity = RotateVector(rb.velocity, angleInDegrees);

        balls.Add(duplicatedBall);

        // Ensure other components are copied over correctly
        duplicatedBall.ballAudio = originalBall.ballAudio;
        duplicatedBall.collisionParticle = Instantiate(originalBall.collisionParticle, duplicatedBall.transform);
        duplicatedBall.collisionParticle.Stop();  
        

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Destroy the duplicated ball
        if (duplicatedBall)
        {
            balls.Remove(duplicatedBall);
            Destroy(duplicatedBall.gameObject);
        }

    }
    private Vector2 RotateVector(Vector2 vector, float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(angleInRadians);
        float sinTheta = Mathf.Sin(angleInRadians);
        float x = vector.x * cosTheta - vector.y * sinTheta;
        float y = vector.x * sinTheta + vector.y * cosTheta;
        return new Vector2(x, y);
    }
    
    public void SpeedUpBall(float duration, float speedUpRatio)
    {
        StartCoroutine(SpeedUpBallForDuration(duration, speedUpRatio));
    }
    private IEnumerator SpeedUpBallForDuration(float duration, float speedUpRatio)
    {
        Vector2 originalVelocity = originalBall.rbgd2D.velocity;

        // Increase the velocity
        originalBall.rbgd2D.velocity = originalVelocity * speedUpRatio;

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Restore the velocity while preserving direction
        Vector2 direction = originalBall.rbgd2D.velocity.normalized;
        originalBall.rbgd2D.velocity = direction * (originalVelocity.magnitude / speedUpRatio);
    }
    
    public void SlowDownBall(float duration, float slowDownRation)
    {
        StartCoroutine(SlowDownBallForDuration(duration, slowDownRation));
    }
    private IEnumerator SlowDownBallForDuration(float duration, float slowDownRatio)
    {
        Vector2 originalVelocity = originalBall.rbgd2D.velocity;
        originalBall.rbgd2D.velocity = originalVelocity * slowDownRatio;
        yield return new WaitForSeconds(duration);
        Vector2 direction = originalBall.rbgd2D.velocity.normalized;
        originalBall.rbgd2D.velocity = direction * (originalVelocity.magnitude * slowDownRatio);

    }
    private void ScoreAction()
    {
        for (int i = balls.Count -1 ; i >= 0; i--)
        {
            if (balls[i].IsClone)
            {
                Destroy(balls[i].gameObject);
                balls.RemoveAt(i);
            }
        }

    }
    private void GameOverAction(int a)
    {
        for (int i = balls.Count -1 ; i >= 0; i--)
        {
            if (balls[i].IsClone)
            {
                Destroy(balls[i].gameObject);
                balls.RemoveAt(i);
            }
        }

    }
}
