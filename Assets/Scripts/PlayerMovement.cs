using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    Vector2 movement;
    public float moveSpeed = 1.0f;

    // Speichert den NPC, mit dem der Spieler interagieren kann
    private IInteractable currentInteractable; 

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // *NEU* - Interaktions-Logik:
        if (Input.GetKeyDown(KeyCode.E)) // Du kannst jede gewünschte Taste wählen (z.B. E)
        {
            // Prüft, ob ein interagierbares Objekt in Reichweite ist
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                currentInteractable.Interact();
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // NEUE METHODE: Betreten des Erkennungsradius (Trigger)
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