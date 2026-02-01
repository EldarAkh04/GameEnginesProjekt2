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

    // --- NEU: Audio Variablen ---
    [Header("Audio Einstellungen")]
    public AudioSource audioSource; // Zieh hier die AudioSource rein
    public AudioClip voiceSound;    // Dein kurzes "Beep" oder "Blip"
    [Range(0.5f, 1.5f)]
    public float minPitch = 0.8f;   // Tiefste Tonlage
    [Range(0.5f, 1.5f)]
    public float maxPitch = 1.2f;   // Höchste Tonlage
    // ----------------------------

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // --- NEU: AudioSource automatisch finden (falls vergessen) ---
    private void Start()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    // -------------------------------------------------------------

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueData == null || dialoguePanel == null) return;
        
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
           StopAllCoroutines();
           dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
           isTyping = false;
           return; 
        }

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
            
            // --- NEU: Sound abspielen bei jedem Buchstaben ---
            if(audioSource != null && voiceSound != null)
            {
                // Zufällige Tonhöhe für den "Brabbel"-Effekt
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                // Sound abspielen (PlayOneShot erlaubt Überlappung)
                audioSource.PlayOneShot(voiceSound);
            }
            // -------------------------------------------------

            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines != null && 
           dialogueData.autoProgressLines.Length > dialogueIndex && 
           dialogueData.autoProgressLines[dialogueIndex])
        {
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