using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Wichtig für Image
using TMPro;          // Wichtig für TextMeshPro

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

    void NextLine()
    {
        if(isTyping)
        {
           // Text sofort vervollständigen
           StopAllCoroutines();
           dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
           isTyping = false;
        }
        else
        {
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
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            // Optional: Hier Sound abspielen
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Auto Progress Logik
        if(dialogueData.autoProgressLines != null && 
           dialogueData.autoProgressLines.Length > dialogueIndex && 
           dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
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