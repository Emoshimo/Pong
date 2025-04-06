using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public Rigidbody2D rgbd2D;
    public int id;
    public float moveSpeed = 2f;
    public float aiDeadZone = 0.7f;
    private Vector3 startPosition;
    private int direction = 0;
    private float moveSpeedMultiplier = 1f;
    private bool isGameRunning = false;
    private void Start() 
    {
        startPosition = transform.position;
        GameManager.instance.onReset += ResetPosition;
        GameManager.instance.gameUI.onStartGame += ChangeGameState;
        GameManager.instance.onGameEnds += OnGameEnd;
    }

    private void ResetPosition() 
    {
        transform.position = startPosition;
    }

    void Update()
    {
        if (isGameRunning)
        {
            if (IsAi())
            {
                // call ai move 
                MoveAi();
            }
            else 
            {
                float value = ProcessInput();
                Move(value);
            }
        }
        

    }

    public float GetHeight()
    {
        return transform.localScale.y;
    }

    private bool IsAi() 
    {
        bool IsPlayer1Ai = IsLeftPaddle() && GameManager.instance.IsPlayer1Ai();
        bool IsPlayer2Ai = !IsLeftPaddle() && GameManager.instance.IsPlayer2Ai();
        return IsPlayer1Ai || IsPlayer2Ai;
    }
    
    private void MoveAi()
    {
        Vector2 ballPos = GameManager.instance.ball.transform.position;
        if (Mathf.Abs(ballPos.y - transform.position.y) > aiDeadZone)
            direction = ballPos.y > transform.position.y ? 1 : -1;

        if (Random.value < 0.02f)
        {
            moveSpeedMultiplier = Random.Range(0.6f, 1.8f);
        }
        Move(direction); 
    }

    private float ProcessInput()
    {
        float movement = 0f;
        if (IsLeftPaddle())
        {
            movement = Input.GetAxis("MovePlayer1");
        }
        else 
        {
            movement = Input.GetAxis("MovePlayer2");

        }

        return movement;
    }
    private void Move(float value)
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
    }

    public bool IsLeftPaddle()
    {
        return id == 1;
    }
    
}


