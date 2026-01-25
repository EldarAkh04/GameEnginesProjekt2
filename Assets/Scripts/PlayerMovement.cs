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

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyHealth normalHealth = enemy.GetComponent<EnemyHealth>();
            if (normalHealth != null)
            {
                normalHealth.TakeDamage(attackDamage, transform);
            }
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

    // Das ist der Timer, der gefehlt hat!
    IEnumerator EndAttackDelay()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }
}