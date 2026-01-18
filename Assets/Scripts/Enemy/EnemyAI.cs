using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2.0f;
    public float checkRadius = 5.0f; // How close player must be to start chasing
    public float attackRadius = 0.3f; // How close to stop and attack

    [Header("Attack Settings")]
    public int damage = 1;     // How much damage to deal
    public float attackCooldown = 1.5f; // How many seconds between attacks
    private float lastAttackTime;    // Timer to track cooldown

    [Header("References")]
    public Transform player; // Drag Player here or let script find it
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator anim; // If you have animations later
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-find player if you forgot to drag it in
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if(player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 2. Only chase if inside the radius but outside attack range
        if (distance <= checkRadius && distance > attackRadius)
        {
            Vector3 direction = player.position - transform.position;
            direction.Normalize(); // Fixes diagonal speed issues
            movement = direction;
            
            // Simple Sprite Flip (Optional)
            if (direction.x < 0) spriteRenderer.flipX = true; // Face Left
            else spriteRenderer.flipX = false; // Face Right
        } else if (distance <= attackRadius)
        {
            movement = Vector2.zero; 

            if (Time.time > lastAttackTime + attackCooldown)
            {
                player.GetComponent<Health>().TakeDamage(damage);
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
        moveCharacter(movement);
    }

    void moveCharacter(Vector2 direction)
    {
        // rb.MovePosition is better for physics (collisions) than transform.Translate
        rb.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    // VISUALIZES THE RANGES IN THE EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
