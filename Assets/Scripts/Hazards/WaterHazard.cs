using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHazard : GridHazard
{
    [Header("Water Properties")]
    public float speedDecreaseFactor = 0.6f;  // Slow the ball by 40%
    public float paddleSlipperyDuration = 3f; // How long paddle becomes "slippery"
    public float paddleSlipperyFactor = 1.5f; // How much more slippery (higher = more slippery)
    
    private Dictionary<Paddle, Coroutine> activePaddleEffects = new Dictionary<Paddle, Coroutine>();

    protected override void Awake()
    {
        base.Awake();
        hazardName = "Water Hazard";
        hazardColor = new Color(0f, 0.5f, 1f); // Blue
    }

    public override void ApplyBallEffect(GameObject ballObj)
    {
        Ball ball = ballObj.GetComponent<Ball>();
        if (ball != null)
        {
            // Decrease ball speed
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Get current velocity and decrease it
                Vector2 currentVelocity = rb.velocity;
                rb.velocity = currentVelocity * speedDecreaseFactor;
                
                // Visual effect
                if (effectParticles != null)
                {
                    ParticleSystem.MainModule main = effectParticles.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0.5f, 1f, 0.7f));
                    effectParticles.Play();
                }
            }
            
            // You could also add a visual effect to the ball
            // e.g., add water dripping particles
        }
    }
    

    
    private IEnumerator CreateWaterSplash(Vector3 position)
    {
        // Here you would instantiate a water splash particle effect
        // For example:
        GameObject splash = null;
        if (effectParticles != null)
        {
            splash = Instantiate(effectParticles.gameObject, position, Quaternion.identity);
            ParticleSystem splashParticles = splash.GetComponent<ParticleSystem>();
            splashParticles.Play();
        }
        
        yield return new WaitForSeconds(1.5f);
        
        if (splash != null)
        {
            Destroy(splash);
        }
    }
    
    public override void Remove()
    {
        // Cancel all active coroutines
        foreach (var pair in activePaddleEffects)
        {
            if (pair.Value != null)
            {
                StopCoroutine(pair.Value);
            }
        }
        activePaddleEffects.Clear();
        
        base.Remove();
    }
}