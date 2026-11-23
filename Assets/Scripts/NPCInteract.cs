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
            if(!dialogueScript.gameObject.activeInHierarchy)
            {
                // Remove E temporarily
                interactText.text = "";

                // Start dialogue
                dialogueScript.TriggerDialogue();
            }
            else
            {
                dialogueScript.AdvanceDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Added tag to Soldier component to make this work
        if(collision.CompareTag("Player"))
        {
            interactText.text = "E";
            playerInRange = true;
            Debug.Log("player in collider");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Added tag to Soldier component to make this work
        if(collision.CompareTag("Player"))
        {
            interactText.text = "";
            playerInRange = false;
            Debug.Log("player exited collider");

            // Close dialogue if open
            dialogueScript.gameObject.SetActive(false);
        }
    }
}