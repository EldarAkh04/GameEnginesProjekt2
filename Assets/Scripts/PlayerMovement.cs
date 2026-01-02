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

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (ShopController.Instance != null && ShopController.Instance.shopPanel.activeSelf)
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                currentInteractable.Interact();
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüft, ob das Objekt ein IInteractable (also unser NPC) ist
        IInteractable interactable = other.GetComponent<IInteractable>();
        
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log("Interaktion in Reichweite! Drücke E zum Interagieren.");
        }
    }

    // NEUE METHODE: Verlassen des Erkennungsradius (Trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        // Wenn der Spieler den Radius verlässt, wird die Referenz gelöscht
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Interaktion verlassen.");
        }
    }
}
