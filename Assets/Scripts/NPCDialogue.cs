using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    
    [TextArea(3, 10)] // Macht das Textfeld im Editor größer
    public string[] dialogueLines;
    
    public bool[] autoProgressLines;
    
    // Hier war der Tippfehler (Progtess -> Progress)
    public float autoProgressDelay = 1.5f; 
    public float typingSpeed = 0.05f;
    
    public AudioClip voiceSound;
    public float voicePitch = 1f;
}