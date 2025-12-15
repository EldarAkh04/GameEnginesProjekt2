using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextLevelName;
    
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        // Prüft, ob der Spieler den Trigger betritt
        //if (other.CompareTag("Player"))

        GameManager.instance.ChangeLevelTo(nextLevelName);
    }
}
