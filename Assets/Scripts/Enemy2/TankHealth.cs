using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [Header("Lebens-Einstellungen")]
    public int maxHealth = 50; // Tank hat mehr Leben!
    private int currentHealth;

    [Header("Health Bar UI")]
    public Image healthBarFill; 
    public GameObject healthCanvas; 

    [Header("Feedback")]
    public SpriteRenderer spriteRenderer; 
    public Color hitColor = Color.red;    
    private Color originalColor;

    // ACHTUNG: Hier referenzieren wir jetzt die TankAI!
    private TankAI tankAI;

    void Start()
    {
        currentHealth = maxHealth;
        
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null) originalColor = spriteRenderer.color;

        // Hier holen wir uns das Tank-Script
        tankAI = GetComponent<TankAI>();

        UpdateHealthBar(); 
    }

    public void TakeDamage(int damage, Transform damageSource = null)
    {
        currentHealth -= damage;
        StartCoroutine(FlashEffect());

        if(tankAI != null && damageSource != null)
        {
            Vector2 knockbackDir = (transform.position - damageSource.position).normalized;
            // Hier rufen wir die Funktion beim Tank auf
            tankAI.ApplyKnockback(knockbackDir);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashEffect()
    {
        if(spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth; 
        }
    }

    void Die()
    {
        if(healthCanvas != null) healthCanvas.SetActive(false);
        Destroy(gameObject);
    }

    public void SetHealth(int amount)
    {
        currentHealth = amount;
        maxHealth = amount; 
        UpdateHealthBar();
    }
}