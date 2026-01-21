using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Wichtig für die Health Bar

public class EnemyHealth : MonoBehaviour
{
    [Header("Lebens-Einstellungen")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Health Bar UI")]
    public Image healthBarFill; // Hier ziehen wir das Grüne Image rein
    public GameObject healthCanvas; // Das ganze Canvas, um es bei Tod auszublenden

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar(); // Setzt den Balken am Anfang auf 100%
    }

    // Diese Funktion wird vom Player aufgerufen
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Gegner Leben: " + currentHealth);

        // Animation für Treffer könnte hier hin (z.B. rotes Blinken)

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // Berechnet den Anteil (z.B. 50 / 100 = 0.5)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth; 
        }
    }

    void Die()
    {
        Debug.Log("Gegner gestorben!");
        
        // Hier Todes-Animation abspielen, Loot droppen, etc.

        // Gegner zerstören
        Destroy(gameObject);
    }
}