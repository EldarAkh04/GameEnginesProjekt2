using UnityEngine;

// Dies ist ein Interface. Es schreibt vor, welche Methoden ein Skript haben muss.
public interface IInteractable
{
    void Interact();
    bool CanInteract();
}