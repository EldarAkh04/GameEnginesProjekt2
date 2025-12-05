using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel; 
    public TextMeshProUGUI dialogueText, nameText;
    public Image portraitImage; 

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        // Sicherstellen, dass alles zugewiesen ist
        if(dialogueData == null || dialoguePanel == null) return;
        
        // Nicht interagieren, wenn Spiel pausiert ist (außer Dialog läuft schon)
        if(PauseController.IsGamePaused && !isDialogueActive) return;

        if(!isDialogueActive)
        {
            StartDialogue(); 
        }
        else
        {
            NextLine();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        
        if(nameText != null) nameText.SetText(dialogueData.npcName);
        if(portraitImage != null) portraitImage.sprite = dialogueData.npcPortrait;
        
        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        StartCoroutine(TypeLine());
    }

    // Ausschnitt aus NPC.cs
    void NextLine()
    {
        if(isTyping)
        {
           // Wenn der Text noch getippt wird, Text sofort vervollständigen
           StopAllCoroutines();
           dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
           isTyping = false;
           // WICHTIG: Die Methode ist hier beendet. Der nächste Klick geht zur nächsten Zeile.
           return; 
        }

        // NUR wenn das Tippen bereits beendet ist: zum nächsten Satz gehen.
        dialogueIndex++; 

        if(dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }
    

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            
            // WICHTIGE ÄNDERUNG:
            // Wir benutzen WaitForSecondsRealtime statt WaitForSeconds.
            // Das funktioniert auch, wenn Time.timeScale auf 0 ist (Pause).
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Auto Progress Logik
        if(dialogueData.autoProgressLines != null && 
           dialogueData.autoProgressLines.Length > dialogueIndex && 
           dialogueData.autoProgressLines[dialogueIndex])
        {
            // Auch hier Realtime nutzen, sonst hängt es beim Auto-Progress
            yield return new WaitForSecondsRealtime(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
    }
}