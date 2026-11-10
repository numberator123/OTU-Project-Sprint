using UnityEngine;
using TMPro; // for adding text to screen above NPC
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    bool playerInRange = false;
    public TMP_Text interactText;

    public Dialogue dialogueScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            dialogueScript.TriggerDialogue();
        }
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