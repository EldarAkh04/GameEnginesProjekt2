using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    // Diese Variable merkt sich, ob das Spiel pausiert ist
    public static bool IsGamePaused = false;

    // Diese Funktion wird vom NPC-Skript aufgerufen
    public static void SetPause(bool shouldPause)
    {
        IsGamePaused = shouldPause;

        if (shouldPause)
        {
            Time.timeScale = 0f; // Zeit anhalten (Spiel friert ein)
        }
        else
        {
            Time.timeScale = 1f; // Zeit weiterlaufen lassen
        }
    }
}