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

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        rb.velocity = movement * moveSpeed;
        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);

        if (Input.GetKeyDown(KeyCode.E)) // Du kannst jede gewünschte Taste wählen (z.B. E)
        {
            // Prüft, ob ein interagierbares Objekt in Reichweite ist
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
            Debug.Log("NPC in Reichweite! Drücke E zum Interagieren.");
        }
    }

    // NEUE METHODE: Verlassen des Erkennungsradius (Trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        // Wenn der Spieler den Radius verlässt, wird die Referenz gelöscht
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("NPC verlassen.");
        }
    }
}
