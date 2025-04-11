using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public Rigidbody2D rgbd2D;
    public int id;
    public float moveSpeed = 2f;
    private Vector3 startPosition;
    private float moveSpeedMultiplier = 1f;
    private bool isGameRunning = false;
    
    // Add controller reference that can be either player or AI
    private IPaddleController controller;
    
    private void Start() 
    {
        startPosition = transform.position;
        GameManager.instance.onReset += ResetPosition;
        GameManager.instance.gameUI.onStartGame += ChangeGameState;
        GameManager.instance.onGameEnds += OnGameEnd;
        GameManager.instance.onGamePaused += OnGamePaused;

        // Set up appropriate controller based on paddle and game mode
        SetupController();
    }
    private void OnGamePaused(bool isPaused)
    {
        // Disable physics when paused
        rgbd2D.simulated = !isPaused;
        
        // Also notify controller
        if (controller is AIPaddleController aiController)
        {
            if (isPaused)
                aiController.OnGamePaused();
            else
                aiController.OnGameResumed();
        }
    }
    private void SetupController()
    {
        bool isLeftPaddle = IsLeftPaddle();
        bool shouldUseAI = (isLeftPaddle && GameManager.instance.IsPlayer1Ai()) || 
                         (!isLeftPaddle && GameManager.instance.IsPlayer2Ai());
        
        if (shouldUseAI)
        {
            // Check if we already have an AI controller
            AIPaddleController existingAI = GetComponent<AIPaddleController>();
            if (existingAI == null)
            {
                // Add AI controller component
                controller = gameObject.AddComponent<AIPaddleController>();
            }
            else
            {
                controller = existingAI;
            }
        }
        else
        {
            // Create player controller
            controller = isLeftPaddle ? 
                new PlayerPaddleController("MovePlayer1") : 
                new PlayerPaddleController("MovePlayer2");
        }
        
        // Initialize the controller with this paddle
        controller.Initialize(this);
    }

    private void ResetPosition() 
    {
        transform.position = startPosition;
    }

    void Update()
    {
        if (isGameRunning && controller != null)
        {
            // Get movement input from the controller
            float moveInput = controller.GetMovementInput();
            Move(moveInput);
        }
    }

    public float GetHeight()
    {
        return transform.localScale.y;
    }
    
    public void SetMoveSpeedMultiplier(float multiplier)
    {
        moveSpeedMultiplier = multiplier;
    }

    public void Move(float value)
    {
        Vector2 velo = rgbd2D.velocity;
        velo.y = moveSpeed * moveSpeedMultiplier * value;
        rgbd2D.velocity = velo;
    }
    
    private void ChangeGameState() 
    {
        isGameRunning = !isGameRunning;
    }
    
    private void OnGameEnd(int winnerId)
    {
        isGameRunning = false;
        ResetPosition();
    }
    
    private void OnDestroy()
    {
        GameManager.instance.onReset -= ResetPosition;
        GameManager.instance.gameUI.onStartGame -= ChangeGameState;
        GameManager.instance.onGameEnds -= OnGameEnd;
        GameManager.instance.onGamePaused -= OnGamePaused;

    }

    public bool IsLeftPaddle()
    {
        return id == 1;
    }
    
    // Method for skill usage
    public void UseSkill(int skillIndex)
    {
        if (SkillManager.instance != null)
        {
            // Get the skill list for this paddle
            List<Skill> skills = id == 1 ? 
                SkillManager.instance.leftPaddleSkills : 
                SkillManager.instance.rightPaddleSkills;
                
            // Activate the skill
            if (skillIndex < skills.Count)
            {
                skills[skillIndex].Activate(id);
            }
        }
    }
    public void UpdateAIDifficulty(float newDifficultyFactor, bool useSkills)
    {
        AIPaddleController aiController = controller as AIPaddleController;
        if (aiController != null)
        {
            aiController.difficultyFactor = newDifficultyFactor;
            aiController.useSkills = useSkills;
            
            // Recalculate dependent parameters
            aiController.reactionDelay = Mathf.Lerp(0.3f, 0.05f, newDifficultyFactor);
        }
    }
}