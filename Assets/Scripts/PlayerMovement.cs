using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public int attackDamage; // <--- NEU: Hier kannst du den Schaden einstellen!
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
        if (rb == null || animator == null) return;
        bool isShopOpen = false;
        if (ShopController.Instance != null && ShopController.Instance.shopPanel != null)
        {
            isShopOpen = ShopController.Instance.shopPanel.activeSelf;
        }

        if (isShopOpen)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat(horizontal, 0);
            animator.SetFloat(vertical, 0);
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractable?.Interact();
            }
            return;
        }
        movement.Set(InputManager.Movement.x, InputManager.Movement.y); 
        rb.velocity = movement * moveSpeed;
        
        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Interagieren

        if (Input.GetKeyDown(KeyCode.E)) // Du kannst jede gewünschte Taste wählen (z.B. E)
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
        // 1. Angriff markieren
        isAttacking = true;
        StartCoroutine(EndAttackDelay());
        animator.SetTrigger("Attack");

        // 2. Alle Gegner im Kreis finden
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Schaden austeilen
        foreach(Collider2D enemy in hitEnemies)
        {
            // --- VERSUCH 1: Ist es ein normaler Gegner? ---
            EnemyHealth normalHealth = enemy.GetComponent<EnemyHealth>();
            if (normalHealth != null)
            {
                normalHealth.TakeDamage(attackDamage, transform);
            }

           /*  // --- VERSUCH 2: Ist es ein Tank? ---
            EnemyHealth tankHealth = enemy.GetComponent<TankHealth>();
            if (tankHealth != null)
            {
                tankHealth.TakeDamage(attackDamage, transform);
            }*/
        } 
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
            Debug.Log("Interaktion in Reichweite! Drücke E zum Interagieren.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Interaktion verlassen.");
        }
    }

    // Das ist der Timer, der gefehlt hat!
    IEnumerator EndAttackDelay()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }
}