using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Lebens-Einstellungen")]
    public int maxHealth;
    private int currentHealth;

    [Header("Health Bar UI")]
    public Image healthBarFill; 
    public GameObject healthCanvas; 

    [Header("Feedback")]
    public SpriteRenderer spriteRenderer; 
    public Color hitColor = Color.red;    
    private Color originalColor;

    [Header("Loot")]
    public List<ItemDropper> itemDrop = new List<ItemDropper>();
    private EnemyAI enemyAI;

    void Start()
    {
        Debug.Log(gameObject.name + " startet mit " + maxHealth + " HP");
        currentHealth = maxHealth;
        
        if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null) originalColor = spriteRenderer.color;

        // Hier holen wir uns das Tank-Script
        enemyAI = GetComponent<EnemyAI>();

        UpdateHealthBar(); 
    }

    public void TakeDamage(int damage, Transform damageSource = null)
    {
        currentHealth -= damage;
        StartCoroutine(FlashEffect());

        if(damageSource != null)
        {
            Vector2 knockbackDir = (transform.position - damageSource.position).normalized;
            if (enemyAI != null)
            {
                enemyAI.ApplyKnockback(knockbackDir);
            }
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
        foreach(ItemDropper lootItem in itemDrop)
        {
            if(Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPref);
            }
        }
        if(healthCanvas != null) healthCanvas.SetActive(false);
        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if(loot != null)
        {
            GameObject droppedItem = Instantiate(loot, transform.position, Quaternion.identity);
        }
    }
}

[System.Serializable]
public class ItemDropper
{
    public GameObject itemPref;
    [Range(0, 100)] public float dropChance;
}