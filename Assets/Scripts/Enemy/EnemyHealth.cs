using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Lebens-Einstellungen")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Health Bar UI")]
    public Image healthBarFill; 
    public GameObject healthCanvas; 

    [Header("Feedback")]
    public SpriteRenderer spriteRenderer; // Ziehe hier den SpriteRenderer des Gegners rein
    public Color hitColor = Color.red;    // Farbe beim Treffer
    private Color originalColor;

    // Referenz zum AI Script für Knockback
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        
        // 1. SpriteRenderer finden, falls nicht zugewiesen
        if(spriteRenderer == null) 
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 2. JETZT die Farbe speichern (ganz wichtig!)
        if(spriteRenderer != null) 
        {
            originalColor = spriteRenderer.color;
        }

        enemyAI = GetComponent<EnemyAI>();

        UpdateHealthBar(); 
    }

    public void TakeDamage(int damage, Transform damageSource = null)
    {
        currentHealth -= damage;
        
        // 1. Visueller Flash (Aufleuchten)
        StartCoroutine(FlashEffect());

        // 2. Rückstoß (Knockback) auslösen
        if(enemyAI != null && damageSource != null)
        {
            // Berechne Richtung: Weg vom Spieler (damageSource)
            Vector2 knockbackDir = (transform.position - damageSource.position).normalized;
            enemyAI.ApplyKnockback(knockbackDir);
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

        // Farbe ändern
        spriteRenderer.color = hitColor;
        // Kurz warten (0.1 Sekunden)
        yield return new WaitForSeconds(0.1f);
        // Farbe zurücksetzen
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
        // Hier könntest du Loot droppen (z.B. eine Münze instanzieren)
        // Instantiate(coinPrefab, transform.position, Quaternion.identity);

        // UI ausblenden
        if(healthCanvas != null) healthCanvas.SetActive(false);
        
        // Gegner zerstören
        Destroy(gameObject);
    }

    // Neue Hilfsfunktion für den Spawner
    public void SetHealth(int amount)
    {
        currentHealth = amount;
        maxHealth = amount; // Sicherheitshalber MaxHealth auch anpassen
        UpdateHealthBar();
    }
}