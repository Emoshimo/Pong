using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridHazard : MonoBehaviour
{
    [Header("Hazard Properties")]
    public string hazardName = "Base Hazard";
    public float duration = 5.0f;
    public bool isPermanent = false;
    public Color hazardColor = Color.white;
    
    // Visual elements
    public SpriteRenderer spriteRenderer;
    public ParticleSystem effectParticles;
    
    // References
    public GridCell parentCell;
    
    // Timing
    protected float startTime;
    protected bool isActive = false;
    
    protected virtual void Awake()
    {
        // Set up visual elements if needed
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
        if (effectParticles == null)
            effectParticles = GetComponentInChildren<ParticleSystem>();
    }
    
    // Initialize the hazard with a grid cell and duration
    public virtual void Initialize(GridCell cell, float durationOverride = -1)
    {
        parentCell = cell;
        
        if (durationOverride > 0)
        {
            duration = durationOverride;
        }
        
        transform.position = cell.position;
        transform.localScale = new Vector3(cell.size.x, cell.size.y, 1);
        
        startTime = Time.time;
        isActive = true;
        
        // Set up visual appearance
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(hazardColor.r, hazardColor.g, hazardColor.b, 0.5f);
        }
        
        // Play particles if available
        if (effectParticles != null)
        {
            effectParticles.Play();
        }
        
        // Start checking for expiration
        if (!isPermanent)
        {
            StartCoroutine(ExpireAfterDuration());
        }
    }
    
    // Check if hazard has expired
    public bool IsExpired()
    {
        if (isPermanent) return false;
        return !isActive || Time.time > startTime + duration;
    }
    
    // Get remaining duration
    public float GetRemainingDuration()
    {
        if (isPermanent) return float.MaxValue;
        return Mathf.Max(0, (startTime + duration) - Time.time);
    }
    
    public virtual void ApplyBallEffect(GameObject ball)
    {
        // Override in derived classes
        Debug.Log($"Ball entered {hazardName} hazard");
    }
    
    // Apply effect to paddle when it enters this hazard
    public virtual void ApplyPaddleEffect(GameObject paddle, int paddleId)
    {
        // Override in derived classes
        Debug.Log($"Paddle {paddleId} entered {hazardName} hazard");
    }
    
    // Remove this hazard (called when expired or manually cleared)
    public virtual void Remove()
    {
        isActive = false;
        
        if (parentCell != null && parentCell.activeHazard == this)
        {
            parentCell.ClearHazard();
        }
        
        // Optional effects before destruction
        if (effectParticles != null)
        {
            var main = effectParticles.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Destroy;
            effectParticles.Stop();
            
            // Detach particles from parent so they can finish playing
            effectParticles.transform.SetParent(null);
        }
        
        // Destroy after a small delay to allow effects to finish
        Destroy(gameObject, 0.5f);
    }
    
    // Coroutine to automatically expire after duration
    protected IEnumerator ExpireAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        if (isActive)
        {
            Remove();
        }
    }
    
    // Handle ball collisions
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        
        // Check if it's a ball
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            ApplyBallEffect(other.gameObject);
            return;
        }
        
        // Check if it's a paddle
        Paddle paddle = other.GetComponent<Paddle>();
        if (paddle != null)
        {
            ApplyPaddleEffect(other.gameObject, paddle.id);
        }
    }
    
    // Handle ball staying in trigger
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        // Override in derived classes if needed
    }
    
    // Handle ball exiting trigger
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // Override in derived classes if needed
    }
}