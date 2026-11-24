using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueTextBox;
    public List<string> lines;
    public float textSpeed = 0.03f;
    public int index;

    [Header("Keyboard Input Settings")]
    public bool isPlayerTyping = false;
    private string currentTypedMessage = "";
    public Action<string> onPlayerSubmit; // Connects to NPCInteract

    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // Listen to raw keyboard character typing
        if (Keyboard.current != null)
            Keyboard.current.onTextInput += OnTextInput;
    }

    void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
    }

    public void TriggerDialogue()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        isPlayerTyping = false;
        currentTypedMessage = "";

        lines.Clear();
        dialogueTextBox.text = string.Empty;
        lines.Add("Hi! I am an NPC, Ask me anything!");

        index = 0;
        StartDialogue();
    }

    private void OnTextInput(char c)
    {
        if (!isPlayerTyping) return;

        // Ignore control characters like Enter (\r, \n) or Backspace (\b)
        if (char.IsControl(c)) return;

        currentTypedMessage += c;
        dialogueTextBox.text = "> " + currentTypedMessage + "_";
    }

    void Update()
    {
        if (isPlayerTyping)
        {
            // Handle Backspace
            if (Keyboard.current.backspaceKey.wasPressedThisFrame && currentTypedMessage.Length > 0)
            {
                currentTypedMessage = currentTypedMessage.Substring(0, currentTypedMessage.Length - 1);
                dialogueTextBox.text = "> " + currentTypedMessage + "_";
            }

            // Handle Enter to submit
            if (Keyboard.current.enterKey.wasPressedThisFrame && !string.IsNullOrWhiteSpace(currentTypedMessage))
            {
                string submittedText = currentTypedMessage.Trim();

                // Check if the player typed "quit" (case-insensitive)
                if (submittedText.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    isPlayerTyping = false;
                    currentTypedMessage = "";
                    dialogueTextBox.text = string.Empty;
                    gameObject.SetActive(false);
                    return;
                }

                isPlayerTyping = false;
                currentTypedMessage = "";
                dialogueTextBox.text = "Thinking...";

                // Send message to NPCInteract -> Backend
                onPlayerSubmit?.Invoke(submittedText);
            }


            // Press Escape to close dialogue
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isPlayerTyping = false;
                gameObject.SetActive(false);
            }
            return;
        }

        // Advance NPC lines with Space, Enter, or Left Click
        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (dialogueTextBox.text == lines[index])
            {
                nextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueTextBox.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueTextBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void nextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            dialogueTextBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Switch to keyboard input mode once NPC finishes speaking
            isPlayerTyping = true;
            currentTypedMessage = "";
            dialogueTextBox.text = "> _";
        }
    }

    // Call this from NPCInteract when the backend replies!
    public void DisplayNPCReply(string reply)
    {
        StopAllCoroutines();
        isPlayerTyping = false;
        lines.Clear();
        lines.Add(reply);
        index = 0;
        dialogueTextBox.text = string.Empty;
        StartDialogue();
    }

    public void AdvanceDialogue()
    {
        if (dialogueTextBox.text == lines[index])
        {
            nextLine();
        }
        else
        {
            StopAllCoroutines();
            dialogueTextBox.text = lines[index];
        }
    }
}