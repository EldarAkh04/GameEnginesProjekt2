using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialog Einstellungen")]
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel; 
    public TextMeshProUGUI dialogueText, nameText;
    public Image portraitImage; 

    [Header("Audio Einstellungen")]
    public AudioSource audioSource; 
    public AudioClip voiceSound;    
    
    [Range(0.5f, 2f)]
    public float minPitch = 0.8f;   
    [Range(0.5f, 2f)]
    public float maxPitch = 1.2f;   
    
    // Timer gegen das Echo (0.05 ist sehr schnell, 0.1 ist normal)
    [Range(0.01f, 0.5f)]
    public float soundInterval = 0.08f; 

    private float lastSoundTime;       
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueData == null || dialoguePanel == null) return;
        if(PauseController.IsGamePaused && !isDialogueActive) return;

        if(!isDialogueActive) StartDialogue(); 
        else NextLine();
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
           
           // SOFORT STOPPEN, wenn Spieler den Text überspringt
           if(audioSource != null) audioSource.Stop(); 
           
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

        // Timer zurücksetzen, damit der erste Ton sofort kommt!
        lastSoundTime = -100f; 

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            
            // --- SOUND LOGIK ---
            if(audioSource != null && voiceSound != null)
            {
                // Prüfen, ob genug Zeit vergangen ist (gegen Echo)
                if(Time.unscaledTime - lastSoundTime >= soundInterval)
                {
                    lastSoundTime = Time.unscaledTime;
                    
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    // PlayOneShot garantiert, dass man etwas hört!
                    audioSource.PlayOneShot(voiceSound);
                }
            }
            // -------------------

            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        // --- WICHTIG: Sobald der Text fertig ist, Sound SOFORT abwürgen ---
        if(audioSource != null) 
        {
            audioSource.Stop();
        }

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
        // Sicherstellen, dass Ruhe ist
        if(audioSource != null) audioSource.Stop();
        
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
    }
}