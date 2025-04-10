using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHazard : GridHazard
{
    [Header("Air Properties")]
    public float windForceMagnitude = 5f;   // Strength of wind force
    public Vector2 windDirection = Vector2.right; // Direction of wind
    public float windDirectionChangeInterval = 2f; // How often to change wind direction
    public bool randomizeDirection = true;  // Should wind direction change randomly?
    
    private Coroutine windChangeCoroutine;
    private List<Rigidbody2D> affectedBalls = new List<Rigidbody2D>();

    protected override void Awake()
    {
        base.Awake();
        hazardName = "Air Hazard";
        hazardColor = new Color(0.8f, 0.8f, 1f); // Light blue/white
        
        // Normalize wind direction
        if (windDirection != Vector2.zero)
        {
            windDirection.Normalize();
        }
        else
        {
            windDirection = Vector2.right;
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        // Add ball to affected list
        Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
        if (ballRb != null && !affectedBalls.Contains(ballRb))
        {
            affectedBalls.Add(ballRb);
        }
    }
    
    protected override void OnTriggerExit2D(Collider2D other)
    {
        // Remove ball from affected list
        Rigidbody2D ballRb = other.GetComponent<Rigidbody2D>();
        if (ballRb != null && affectedBalls.Contains(ballRb))
        {
            affectedBalls.Remove(ballRb);
        }
    }

    public override void Initialize(GridCell cell, float durationOverride = -1)
    {
        base.Initialize(cell, durationOverride);
        
        // Start changing wind direction
        if (randomizeDirection && windChangeCoroutine == null)
        {
            windChangeCoroutine = StartCoroutine(ChangeWindDirection());
        }
        
        // Start applying force to balls
        StartCoroutine(ApplyWindForce());
    }
    
    private IEnumerator ChangeWindDirection()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(windDirectionChangeInterval);
            
            if (randomizeDirection && isActive)
            {
                // Generate random angle
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                windDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                
                // Update particle effect if available
                if (effectParticles != null)
                {
                    var main = effectParticles.main;
                    var velocityOverLifetime = effectParticles.velocityOverLifetime;
                    velocityOverLifetime.x = windDirection.x * windForceMagnitude;
                    velocityOverLifetime.y = windDirection.y * windForceMagnitude;
                    
                    // Rotate the particle system to match wind direction
                    float rotationAngle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;
                    effectParticles.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
                }
            }
        }
    }
    
    private IEnumerator ApplyWindForce()
    {
        while (isActive)
        {
            // Apply force to all balls in the affected list
            foreach (Rigidbody2D ballRb in new List<Rigidbody2D>(affectedBalls))
            {
                if (ballRb != null)
                {
                    // Apply wind force
                    ballRb.AddForce(windDirection * windForceMagnitude, ForceMode2D.Force);
                    
                    // Optional: Add slight curve to ball path
                    Vector2 perpendicular = new Vector2(-windDirection.y, windDirection.x);
                    ballRb.AddTorque(Random.Range(-0.5f, 0.5f) * windForceMagnitude, ForceMode2D.Force);
                }
                else
                {
                    // Remove null references
                    affectedBalls.Remove(ballRb);
                }
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
    
    public override void ApplyBallEffect(GameObject ballObj)
    {
        // This is handled continuously in the ApplyWindForce coroutine
        // Additional one-time effects could be added here
        Ball ball = ballObj.GetComponent<Ball>();
        if (ball != null)
        {
            // Maybe create a small wind burst effect
            if (effectParticles != null)
            {
                var burst = effectParticles.emission;
                burst.SetBursts(new ParticleSystem.Burst[] { 
                    new ParticleSystem.Burst(0f, 10)
                });
            }
        }
    }
    
    public override void ApplyPaddleEffect(GameObject paddleObj, int paddleId)
    {
        // Air doesn't affect paddles in this implementation
        // Could add slight paddle movement resistance if desired
    }
    
    public override void Remove()
    {
        // Stop coroutines
        if (windChangeCoroutine != null)
        {
            StopCoroutine(windChangeCoroutine);
            windChangeCoroutine = null;
        }
        
        // Clear affected balls
        affectedBalls.Clear();
        
        base.Remove();
    }
}