using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHazard : GridHazard
{
    [Header("Fire Properties")]
    public float speedIncreaseFactor = 1.5f;
    public bool leaveTrail = false;
    public GameObject fireTrailPrefab;
    public float damageAmount = 0f; // Optional damage to paddle
    
    private List<GameObject> activeTrails = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        hazardName = "Fire Hazard";
        hazardColor = new Color(1f, 0.3f, 0f); // Orange-red
    }

    public override void ApplyBallEffect(GameObject ballObj)
    {
        Ball ball = ballObj.GetComponent<Ball>();
        if (ball != null)
        {
            // Increase ball speed
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Get current velocity and increase it
                Vector2 currentVelocity = rb.velocity;
                rb.velocity = currentVelocity * speedIncreaseFactor;
                
                // Visual effect
                if (effectParticles != null)
                {
                    ParticleSystem.EmissionModule emission = effectParticles.emission;
                    emission.rateOverTime = emission.rateOverTime.constant * 2f;
                    StartCoroutine(ResetParticles(1f));
                }
                
                // Create trail if enabled
                if (leaveTrail && fireTrailPrefab != null)
                {
                    StartCoroutine(CreateTrail(ball.transform.position));
                }
            }
            
            // You could also add a visual effect to the ball
            // e.g., change its color or add a particle effect
        }
    }
    
    public override void ApplyPaddleEffect(GameObject paddleObj, int paddleId)
    {
        if (damageAmount <= 0) return;
        
        Paddle paddle = paddleObj.GetComponent<Paddle>();
        if (paddle != null)
        {
            // Could reduce paddle size temporarily
            float currentScale = paddle.transform.localScale.y;
            paddle.transform.localScale = new Vector3(
                paddle.transform.localScale.x,
                currentScale * 0.8f, // Reduce by 20%
                paddle.transform.localScale.z
            );
            
            // Reset after delay
            StartCoroutine(ResetPaddleSize(paddle, currentScale, 2f));
        }
    }
    
    private IEnumerator ResetParticles(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effectParticles != null && isActive)
        {
            ParticleSystem.EmissionModule emission = effectParticles.emission;
            emission.rateOverTime = emission.rateOverTime.constant / 2f;
        }
    }
    
    private IEnumerator ResetPaddleSize(Paddle paddle, float originalSize, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (paddle != null)
        {
            paddle.transform.localScale = new Vector3(
                paddle.transform.localScale.x,
                originalSize,
                paddle.transform.localScale.z
            );
        }
    }
    
    private IEnumerator CreateTrail(Vector3 position)
    {
        // Create trail effect
        GameObject trail = Instantiate(fireTrailPrefab, position, Quaternion.identity);
        activeTrails.Add(trail);
        
        // Destroy after delay
        yield return new WaitForSeconds(1.5f);
        
        if (trail != null)
        {
            activeTrails.Remove(trail);
            Destroy(trail);
        }
    }
    
    public override void Remove()
    {
        // Clean up any active trails
        foreach (GameObject trail in activeTrails)
        {
            if (trail != null)
            {
                Destroy(trail);
            }
        }
        activeTrails.Clear();
        
        base.Remove();
    }
}