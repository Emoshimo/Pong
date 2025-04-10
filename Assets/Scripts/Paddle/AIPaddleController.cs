using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour, IPaddleController
{
    [Header("AI Settings")]
    public float reactionDelay = 0.1f;
    public float difficultyFactor = 0.5f; // 0 = easy, 1 = perfect
    public bool useSkills = false;
    public float skillUseFrequency = 0.1f; // Probability to use skill per second
    
    private Paddle paddle;
    private Ball targetBall;
    private Vector2 targetPosition;
    private float lastDecisionTime;
    private float deadZone = 0.1f;
    private Coroutine skillUsageCoroutine;
    private float speedMultiplier = 1.0f;
    
    private void Awake()
    {
        // Make sure we don't duplicate controllers on a paddle
        AIPaddleController[] controllers = GetComponents<AIPaddleController>();
        if (controllers.Length > 1)
        {
            Destroy(this);
        }
    }
    
    public void Initialize(Paddle paddleToControl)
    {
        paddle = paddleToControl;
        
        // Update difficulty settings from LevelManager if available
        if (LevelManager.instance != null)
        {
            LevelData levelData = LevelManager.instance.GetCurrentLevelData();
            if (levelData != null)
            {
                difficultyFactor = levelData.aiDifficulty;
                useSkills = levelData.aiUsesSkills;
            }
        }
        
        // Adjust parameters based on difficulty
        reactionDelay = Mathf.Lerp(0.3f, 0.05f, difficultyFactor);
        
        // Start AI skill usage if enabled
        if (useSkills && skillUsageCoroutine == null)
        {
            skillUsageCoroutine = StartCoroutine(UseSkillsRandomly());
        }
        
        // Occasionally change speed multiplier for more human-like movement
        StartCoroutine(VarySpeedMultiplier());
    }
    
    public float GetMovementInput()
    {
        FindTargetBall();
        
        if (targetBall != null)
        {
            // Only make decisions after delay
            if (Time.time > lastDecisionTime + reactionDelay)
            {
                DecideMovement();
                lastDecisionTime = Time.time;
            }
            
            // Return movement direction to target position
            if (targetPosition.y > paddle.transform.position.y + deadZone)
                return 1f * speedMultiplier;
            else if (targetPosition.y < paddle.transform.position.y - deadZone)
                return -1f * speedMultiplier;
        }
        
        return 0f;
    }
    
    private void FindTargetBall()
    {
        // Find ball closest to paddle that is moving toward the paddle
        Ball[] balls = FindObjectsOfType<Ball>();
        float closestDistance = float.MaxValue;
        targetBall = null;
        bool isPaddleRight = !paddle.IsLeftPaddle();
        
        foreach (Ball ball in balls)
        {
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb == null) continue;
            
            // Check if ball is moving toward paddle
            bool isMovingTowardPaddle = (isPaddleRight && ballRb.velocity.x > 0) ||
                                       (!isPaddleRight && ballRb.velocity.x < 0);
            
            if (isMovingTowardPaddle)
            {
                float distance = Vector2.Distance(transform.position, ball.transform.position);
                
                // Add artificial difficulty - sometimes ignore the closer ball
                if (Random.value > difficultyFactor && balls.Length > 1)
                {
                    distance = Random.Range(distance, distance * 2f);
                }
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetBall = ball;
                }
            }
        }
        
        // If no ball is moving toward paddle, pick any ball
        if (targetBall == null && balls.Length > 0)
        {
            targetBall = balls[Random.Range(0, balls.Length)];
        }
    }
    
    private void DecideMovement()
    {
        if (targetBall == null) return;
        
        Rigidbody2D ballRb = targetBall.GetComponent<Rigidbody2D>();
        if (ballRb == null) return;
        
        // Predict where the ball will be
        Vector2 ballPosition = targetBall.transform.position;
        Vector2 ballVelocity = ballRb.velocity;
        float paddleX = paddle.transform.position.x;
        
        // Calculate time until ball reaches paddle
        float timeToReachPaddle = 0;
        if (Mathf.Abs(ballVelocity.x) > 0.1f) // Avoid division by near-zero
        {
            timeToReachPaddle = Mathf.Abs((paddleX - ballPosition.x) / ballVelocity.x);
        }
        
        if (timeToReachPaddle > 0 && timeToReachPaddle < 5f) // Only predict if ball is coming toward paddle soon
        {
            // Predict y position when ball reaches paddle
            float predictedY = ballPosition.y + (ballVelocity.y * timeToReachPaddle);
            
            // Add error based on difficulty (higher difficulty = less error)
            float maxError = Mathf.Lerp(2f, 0f, difficultyFactor);
            float error = Random.Range(-maxError, maxError);
            
            targetPosition = new Vector2(transform.position.x, predictedY + error);
            
            // Clamp to playable area (assuming ±4.5 is the vertical boundary)
            float maxY = 4.5f;
            targetPosition.y = Mathf.Clamp(targetPosition.y, -maxY, maxY);
        }
        else
        {
            // Ball is moving away or too far, return to center with some randomness
            targetPosition = new Vector2(transform.position.x, Random.Range(-1f, 1f));
        }
    }
    
    private IEnumerator UseSkillsRandomly()
    {
        while (true)
        {
            // Wait random time between skill attempts
            yield return new WaitForSeconds(Random.Range(2f, 8f) / (difficultyFactor + 0.5f));
            
            if (useSkills && SkillManager.instance != null && paddle != null)
            {
                // Get available skills for this paddle
                List<Skill> availableSkills = paddle.id == 1 ? 
                    SkillManager.instance.leftPaddleSkills : 
                    SkillManager.instance.rightPaddleSkills;
                
                if (availableSkills.Count > 0)
                {
                    // Only use skill if target ball exists and is moving toward paddle
                    if (targetBall != null)
                    {
                        Rigidbody2D ballRb = targetBall.GetComponent<Rigidbody2D>();
                        bool isMovingTowardPaddle = false;
                        
                        if (ballRb != null)
                        {
                            isMovingTowardPaddle = (paddle.id == 2 && ballRb.velocity.x > 0) || 
                                                  (paddle.id == 1 && ballRb.velocity.x < 0);
                        }
                        
                        if (isMovingTowardPaddle && Random.value < skillUseFrequency * difficultyFactor)
                        {
                            // Choose a random skill to use
                            int skillIndex = Random.Range(0, availableSkills.Count);
                            if (!availableSkills[skillIndex].IsOnCooldown(paddle.id))
                            {
                                // Use the skill
                                paddle.UseSkill(skillIndex);
                            }
                        }
                    }
                }
            }
        }
    }
    
    private IEnumerator VarySpeedMultiplier()
    {
        while (true)
        {
            // Occasionally change speed to make AI movement more human-like
            speedMultiplier = Random.Range(0.85f, 1.0f);
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        }
    }
    
    private void OnDestroy()
    {
        if (skillUsageCoroutine != null)
        {
            StopCoroutine(skillUsageCoroutine);
        }
    }
    
    public void OnGamePaused()
    {
        // Implement if needed (e.g., stop decision making during pause)
    }
    
    public void OnGameResumed()
    {
        // Implement if needed (e.g., reset decision timer)
        lastDecisionTime = Time.time;
    }
}