using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAI : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 1.0f; // Langsamer als der normale Gegner
    public float checkRadius = 6.0f; 
    public float attackRadius = 0.8f; 

    [Header("Attack Settings")]
    public int damage = 20; // Macht mehr Aua!
    public float attackCooldown = 2.0f; 
    private float lastAttackTime;    

    [Header("Knockback Settings")]
    public float knockbackForce = 3f; // Fliegt kaum weg (Schwer)
    public float knockbackDuration = 0.1f; 
    
    // --- NEU: Die Sperre ---
    public float knockbackCooldown = 1.5f; // Ignoriert Schläge für 1.5 Sekunden
    private float lastKnockbackTime; // Wann wurde er zuletzt geschubst?

    [Header("References")]
    public Transform player; 
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    private bool isKnockedBack = false;
    private bool canMove = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if(player == null || isKnockedBack) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= checkRadius && distance > attackRadius)
        {
            Vector3 direction = player.position - transform.position;
            direction.Normalize(); 
            movement = direction;
            
            if (direction.x < 0) spriteRenderer.flipX = true; 
            else spriteRenderer.flipX = false; 
        } 
        else if (distance <= attackRadius)
        {
            movement = Vector2.zero; 
            
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack && canMove)
        {
            rb.MovePosition((Vector2)transform.position + (movement * speed * Time.fixedDeltaTime));
        }
    }

    void AttackPlayer()
    {
        if(player.GetComponent<Health>() != null)
        {
            player.GetComponent<Health>().TakeDamage(damage);
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        // --- NEU: Die Prüfung ---
        // Wenn der letzte Schubser noch nicht lange her ist -> Abbrechen!
        if (Time.time < lastKnockbackTime + knockbackCooldown)
        {
            return; // Tank ignoriert den Rückstoß und läuft weiter!
        }

        if(isKnockedBack) return; 

        lastKnockbackTime = Time.time; // Zeit merken
        StartCoroutine(KnockbackRoutine(direction));
    }

    IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero; 
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero; 
        isKnockedBack = false;
    }
}