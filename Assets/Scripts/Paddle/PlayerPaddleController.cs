using UnityEngine;

public class PlayerPaddleController : IPaddleController
{
    private string inputAxis;
    private Paddle paddle;

    public PlayerPaddleController(string axis)
    {
        inputAxis = axis;
    }

    public void Initialize(Paddle paddle)
    {
        this.paddle = paddle;
    }

    public float GetMovementInput()
    {
        return Input.GetAxis(inputAxis);
    }
}