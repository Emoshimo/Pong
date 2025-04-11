using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rbgd2D;
    public BallAudio ballAudio;
    public ParticleSystem collisionParticle;
    public float maxInitialAngle = 0.67f;
    public float maxCollisionAngle = 45f;
    public float moveSpeed = 1f;
    public float maxStartY = 3.5f;
    public float speedPaddleMultiplier = 1.1f;
    public float speedWallMultiplier = 1.05f;
    private float startX = 0f;
    private float maxSpeed = 270f;
    public bool IsClone = false;
    public int lastHitPaddleId = -1;

    private void Start() 
    {
        GameManager.instance.onReset += ResetBall;
        GameManager.instance.gameUI.onStartGame += ResetBall;
        GameManager.instance.onGamePaused += OnGamePaused;

    }

    public void ResetBall()
    {
        ResetBallPosition();
        InitialPush();
    }
    private void OnGamePaused(bool isPaused)
    {
        // When paused, disable physics
        rbgd2D.simulated = !isPaused;
    }

    private void ResetBallPosition() {
        float positionY = Random.Range(-maxStartY, maxStartY);
        Vector2 position = new Vector2(startX, positionY);
        transform.position = position;
    }
    private void InitialPush() {

        Vector2 dir = Vector2.left;
        if (Random.value < 0.5f) 
        {
            dir = Vector2.right;
        }
        
        dir.y = Random.Range(-maxInitialAngle, maxInitialAngle);
        rbgd2D.velocity = dir * moveSpeed;    
        EmitParticle(16);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        ScoreZone scoreZone = other.GetComponent<ScoreZone>();
        if (scoreZone != null) 
        {
            GameManager.instance.OnScoreZoneReached(scoreZone.id);
            GameManager.instance.screenShake.StartShake(0.33f, 0.1f);

        }
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        Paddle paddle = other.collider.GetComponent<Paddle>();
        if (paddle)
        {
            ballAudio.PlayPaddleSound();
            if (rbgd2D.velocity.magnitude < maxSpeed)
                rbgd2D.velocity *= speedPaddleMultiplier;
            EmitParticle(12);
            AdjustAngle(paddle, other);
            GameManager.instance.screenShake.StartShake(Mathf.Sqrt(rbgd2D.velocity.magnitude) * 0.02f, 0.075f);
            lastHitPaddleId = paddle.id;
        }       
        
        Wall wall = other.collider.GetComponent<Wall>();
        if (wall)
        {
            ballAudio.PlayWallSound();
            if (rbgd2D.velocity.magnitude < maxSpeed)
                rbgd2D.velocity *= speedWallMultiplier;
            EmitParticle(6);
            GameManager.instance.screenShake.StartShake(0.033f, 0.033f);
        }   
    }

    private void EmitParticle(int amount)
    {
        collisionParticle.Emit(amount);
    }

    private void AdjustAngle(Paddle paddle, Collision2D collision)
    {
        Vector2 median = Vector2.zero;
        foreach (ContactPoint2D point in collision.contacts)
        {
            median += point.point;
            //Debug.DrawRay(point.point, Vector3.right, Color.red, 1f);
        }
        median = median / collision.contactCount;
        //Debug.DrawRay(median, Vector3.right, Color.cyan, 1f);

        // calculate relative distance from center 
        float absoluteDistanceFromCenter = median.y - paddle.transform.position.y;
        float relativeDistanceFromCenter = absoluteDistanceFromCenter * 2 / paddle.GetHeight();
        Debug.Log(relativeDistanceFromCenter);

        int angleSign = paddle.IsLeftPaddle() ? 1 : -1; 
        float angle = relativeDistanceFromCenter * maxCollisionAngle * angleSign;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector2 dir = paddle.IsLeftPaddle()? Vector2.right : Vector2.left;
        Vector2 velocity = rotation * dir * rbgd2D.velocity.magnitude;
        rbgd2D.velocity = velocity;
        Debug.Log(rotation);
    }

    private void OnDestroy() 
    {
        GameManager.instance.onReset -= ResetBall;
        GameManager.instance.gameUI.onStartGame -= ResetBall;
        if (GameManager.instance != null)
        {
            GameManager.instance.onGamePaused -= OnGamePaused;
        }
    }
}

