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
    public int attackDamage;
    private float nextAttackTime = 0f;
    public float attackDuration = 0.4f;
    private bool isAttacking = false;

    // --- NEU: AUDIO VARIABLEN ---
    [Header("Kampf Audio")]
    public AudioSource audioSource; // Die Audio Source Komponente am Player
    public AudioClip attackSound;   // Die Sound-Datei (Wusch/Schlag)
    // ----------------------------

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Falls du vergessen hast, die AudioSource zuzuweisen, suchen wir sie automatisch
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0); 
            return;
        }
        if (rb == null || animator == null) return;
        bool isShopOpen = false;
        if (ShopController.Instance != null && ShopController.Instance.shopPanel != null)
        {
            isShopOpen = ShopController.Instance.shopPanel.activeSelf;
        }

        bool isMenuOpen = MenuController.IsMenuOpen;

        if (isShopOpen || isMenuOpen)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat(horizontal, 0);
            animator.SetFloat(vertical, 0);
            animator.SetFloat("Speed", 0);
            if (isShopOpen && Input.GetKeyDown(KeyCode.E))
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                currentInteractable.Interact();
            }
        }
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
        isAttacking = true;
        StartCoroutine(EndAttackDelay());
        animator.SetTrigger("Attack");

        // --- NEU: SOUND ABSPIELEN ---
        if (audioSource != null && attackSound != null)
        {
            // Zufällige Tonhöhe (Pitch) zwischen 0.9 und 1.1
            // Das sorgt dafür, dass nicht jeder Schlag exakt gleich klingt
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            
            // Sound einmal abspielen
            audioSource.PlayOneShot(attackSound);
        }
        // -----------------------------

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            // Prüfen auf normalen Gegner
            EnemyHealth normalHealth = enemy.GetComponent<EnemyHealth>();
            if (normalHealth != null)
            {
                normalHealth.TakeDamage(attackDamage, transform);
            }
            
            // Falls du den Tank noch hast, hier die Zeilen dafür (einfach Kommentar entfernen):
            /*
            TankHealth tankHealth = enemy.GetComponent<TankHealth>();
            if (tankHealth != null)
            {
                tankHealth.TakeDamage(attackDamage, transform);
            }
            */
        } 
    }

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

    IEnumerator EndAttackDelay()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }
}