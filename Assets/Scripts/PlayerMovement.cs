using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    [SerializeField] public float moveSpeed = 1.0f;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";

    private IInteractable currentInteractable;

    [Header("Kampf Einstellungen")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    // --- NEU: Einstellungen für den Bewegungs-Stopp ---
    public float attackDuration = 0.4f; // Wie lange dauert die Animation? (in Sekunden)
    private bool isAttacking = false;   // Schlägt er gerade zu?
    // --------------------------------------------------

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // --- NEU: Bewegung verhindern, wenn man angreift ---
        if (isAttacking)
        {
            rb.velocity = Vector2.zero; // Sofort anhalten!
            // Optional: Lauf-Animation stoppen, damit er nicht auf der Stelle läuft
            animator.SetFloat("Speed", 0); 
            return; // Hier bricht Update ab -> Keine Bewegung wird berechnet
        }
        // ---------------------------------------------------

        movement.Set(InputManager.Movement.x, InputManager.Movement.y); 
        rb.velocity = movement * moveSpeed;
        
        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Interagieren
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                currentInteractable.Interact();
            }
        }

        // Angreifen
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // --- NEU: Angriff markieren und Timer starten ---
        isAttacking = true;
        StartCoroutine(EndAttackDelay());
        // ------------------------------------------------

        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Wir haben " + enemy.name + " getroffen!");
        }
    }

    // --- NEU: Die Uhr, die wartet, bis der Schlag vorbei ist ---
    IEnumerator EndAttackDelay()
    {
        // Wartet so viele Sekunden, wie du bei attackDuration eingestellt hast
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false; // Jetzt darf er sich wieder bewegen
    }
    // -----------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}