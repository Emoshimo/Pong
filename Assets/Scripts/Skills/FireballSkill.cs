using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSkill : Skill
{
    [Header("Fireball Properties")]
    public float speedBoost = 1.5f;
    public Color fireballColor = new Color(1f, 0.5f, 0f, 0.8f);
    public GameObject fireTrailPrefab;
    public float burnThroughObstacleProbability = 0.7f; // 70% chance to burn through obstacles
    
    private List<Ball> activeBalls = new List<Ball>();
    private List<SpriteRenderer> originalRenderers = new List<SpriteRenderer>();
    private List<Color> originalColors = new List<Color>();
    private List<GameObject> activeTrails = new List<GameObject>();
    private Coroutine trailCoroutine;

    protected override void Start()
    {
        base.Start();
        effectDuration = 5f;   
        skillName = "Fireball";
        skillType = SkillType.Active;
        cooldownTime = 12f;
        manaCost = 25f;
        skillDescription = "Transforms the ball into a fireball for " + effectDuration + 
                          " seconds, increasing its speed and allowing it to burn through obstacles.";
        
        // Load icon if not set
        if (skillIcon == null)
        {
            // Try to load from Resources folder
            skillIcon = Resources.Load<Sprite>("Icons/Fireball");
        }
    }

    protected override void UseAbility(int paddleId)
    {
        // Find all balls in the scene
        Ball[] balls = FindObjectsOfType<Ball>();
        
        foreach (Ball ball in balls)
        {
            // Only affect balls that belong to this paddle or all balls in some cases
            bool shouldAffect = ball.lastHitPaddleId == paddleId || ball.lastHitPaddleId == 0;
            
            if (shouldAffect && !activeBalls.Contains(ball))
            {
                activeBalls.Add(ball);
                
                // Store original color
                SpriteRenderer renderer = ball.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    originalRenderers.Add(renderer);
                    originalColors.Add(renderer.color);
                    
                    // Change ball color to fire
                    renderer.color = fireballColor;
                }
                
                // Increase ball speed
                Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity *= speedBoost;
                }
                
                // Add ability to burn through obstacles
                ball.gameObject.AddComponent<FireballEffect>().Initialize(this, burnThroughObstacleProbability);
            }
        }
        
        // Start trail effect if available
        if (fireTrailPrefab != null && trailCoroutine == null)
        {
            trailCoroutine = StartCoroutine(CreateTrails());
        }
        
        // Start effect duration timer
        StartCoroutine(EffectDurationCoroutine(effectDuration, paddleId));
    }
    
    protected override void EndEffect(int paddleId)
    {
        // Restore original ball properties
        for (int i = 0; i < activeBalls.Count; i++)
        {
            if (activeBalls[i] != null)
            {
                // Remove fireball effect component
                FireballEffect effect = activeBalls[i].GetComponent<FireballEffect>();
                if (effect != null)
                {
                    Destroy(effect);
                }
                
                // Restore original color
                if (i < originalRenderers.Count && originalRenderers[i] != null)
                {
                    originalRenderers[i].color = originalColors[i];
                }
                
                // Ball speed is not reset - it stays at the higher speed
            }
        }
        
        // Clean up lists
        activeBalls.Clear();
        originalRenderers.Clear();
        originalColors.Clear();
        
        // Stop trail coroutine
        if (trailCoroutine != null)
        {
            StopCoroutine(trailCoroutine);
            trailCoroutine = null;
        }
        
        // Clean up trails
        foreach (GameObject trail in activeTrails)
        {
            if (trail != null)
            {
                Destroy(trail);
            }
        }
        activeTrails.Clear();
    }
    
    private IEnumerator CreateTrails()
    {
        while (isEffectActive)
        {
            foreach (Ball ball in activeBalls)
            {
                if (ball != null)
                {
                    // Create fire trail effect
                    GameObject trail = Instantiate(fireTrailPrefab, ball.transform.position, Quaternion.identity);
                    activeTrails.Add(trail);
                    
                    // Set up cleanup
                    StartCoroutine(DestroyTrailAfterDelay(trail, 1f));
                }
            }
            
            yield return new WaitForSeconds(0.1f); // Create trail every 0.1 seconds
        }
    }
    
    private IEnumerator DestroyTrailAfterDelay(GameObject trail, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (trail != null)
        {
            activeTrails.Remove(trail);
            Destroy(trail);
        }
    }
    
    // Helper component to handle fireball-specific effects
    private class FireballEffect : MonoBehaviour
    {
        private FireballSkill parentSkill;
        private float burnThroughChance;
        
        public void Initialize(FireballSkill skill, float burnChance)
        {
            parentSkill = skill;
            burnThroughChance = burnChance;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Handle burning through obstacles
            GridHazard hazard = other.GetComponent<GridHazard>();
            if (hazard != null && Random.value < burnThroughChance)
            {
                // Chance to destroy hazards on contact
                hazard.Remove();
                
                // Create burn effect
                if (parentSkill.fireTrailPrefab != null)
                {
                    GameObject burnEffect = Instantiate(parentSkill.fireTrailPrefab, 
                                                      transform.position, 
                                                      Quaternion.identity);
                    
                    // Make it larger
                    burnEffect.transform.localScale *= 2f;
                    
                    // Destroy after delay
                    Destroy(burnEffect, 1f);
                }
            }
        }
    }
}