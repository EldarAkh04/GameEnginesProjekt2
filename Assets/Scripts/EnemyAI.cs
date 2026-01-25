using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2.0f;
    public float checkRadius = 5.0f; 
    public float attackRadius = 0.8f; // Etwas größer machen, damit er nicht in den Spieler reinkriecht

    [Header("Attack Settings")]
    public int damage = 10;     
    public float attackCooldown = 1.5f; 
    private float lastAttackTime;    

    [Header("Knockback Settings")]
    public float knockbackForce = 10f; // Wie stark fliegt er zurück?
    public float knockbackDuration = 0.2f; // Wie lange kann er sich nicht bewegen?

    [Header("References")]
    public Transform player; 
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    // Zustands-Variablen
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
        // Wenn wir zurückgestoßen werden, darf keine normale Bewegung berechnet werden
        if(player == null || isKnockedBack) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Bewegung berechnen
        if (distance <= checkRadius && distance > attackRadius)
        {
            Vector3 direction = player.position - transform.position;
            direction.Normalize(); 
            movement = direction;
            
            // Sprite Flip
            if (direction.x < 0) spriteRenderer.flipX = true; 
            else spriteRenderer.flipX = false; 
        } 
        else if (distance <= attackRadius)
        {
            // Stop movement to attack
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
        // Nur bewegen, wenn NICHT im Knockback
        if (!isKnockedBack && canMove)
        {
            moveCharacter(movement);
        }
    }

    void moveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * speed * Time.fixedDeltaTime));
    }

    void AttackPlayer()
    {
        // Hier später: Animation abspielen (anim.SetTrigger("Attack"))
        
        // Schaden verursachen
        if(player.GetComponent<Health>() != null)
        {
            player.GetComponent<Health>().TakeDamage(damage);
        }
    }

    // --- NEU: Knockback Funktion ---
    // Diese wird vom EnemyHealth Skript aufgerufen
    public void ApplyKnockback(Vector2 direction)
    {
        if(isKnockedBack) return; // Nicht doppelt machen

        StartCoroutine(KnockbackRoutine(direction));
    }

    IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        
        // Physikalischen Impuls geben (Ruck nach hinten)
        rb.velocity = Vector2.zero; // Aktuelle Bewegung stoppen
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        // Warten (während der Gegner fliegt)
        yield return new WaitForSeconds(knockbackDuration);

        // Wieder normalisieren
        rb.velocity = Vector2.zero; // Rutschen stoppen
        isKnockedBack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}