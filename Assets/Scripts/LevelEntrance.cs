using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntrance : MonoBehaviour
{
    [SerializeField] private string nextLevelName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüft, ob der Spieler den Trigger betritt
        if (other.CompareTag("Player"))
        {
            // Wechselt zum nächsten Level
            GameManager.instance.ChangeLevelTo(nextLevelName);
        }
    }
}
