using System.Collections.Generic;
using UnityEngine;
using TMPro; // for adding text to screen above NPC
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    bool playerInRange = false;
    public TMP_Text interactText;

    public Dialogue dialogueScript;

    [SerializeField] private NPCData data;

    // Runtime only, resets every Play session
    private bool hasMet = false;
    private int timesTalked = 0;

    public bool HasMet => hasMet;
    public int TimesTalked => timesTalked;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Talk();
        }
    }

    private void Talk()
    {
        if(dialogueScript.IsOpen || data == null || !data.canTalk)
        {
            return;
        }

        List<string> lines = data.greetingLines;
        if(hasMet && data.repeatLines.Count > 0)
        {
            lines = data.repeatLines;
        }

        dialogueScript.TriggerDialogue(lines, data.npcName);

        // Nothing opens when the NPC has no lines, so that doesn't count as meeting
        if(!dialogueScript.IsOpen)
        {
            return;
        }

        hasMet = true;
        timesTalked++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // added tag to Solider component to make this work
        if(collision.CompareTag("Player"))
        {
            interactText.text = "E";
            playerInRange = true;
            Debug.Log("player in collider");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // added tag to Solider component to make this work
        if(collision.CompareTag("Player"))
        {
            interactText.text = "";
            playerInRange = false;
            Debug.Log("player exited collider");
        }
    }
}