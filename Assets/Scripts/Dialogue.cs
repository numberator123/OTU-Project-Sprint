using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;



public class Dialogue : MonoBehaviour

{
    public TextMeshProUGUI dialogueTextBox;
    // Optional: shows the speaker's name. Kept out of dialogueTextBox because Update compares that text to lines[index].
    public TextMeshProUGUI nameText;
    public List<string> lines;
    public float textSpeed;
    public int index;

    public bool IsOpen => gameObject.activeSelf;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void TriggerDialogue(List<string> newLines, string speaker)
    {
        if (newLines == null || newLines.Count == 0)
        {
            return;
        }

        StopAllCoroutines();
        gameObject.SetActive(true);

        // Copy so clearing or editing these lines never touches the NPC's data asset
        lines = new List<string>(newLines);
        dialogueTextBox.text = string.Empty;

        if (nameText != null)
        {
            nameText.text = speaker;
        }

        index = 0;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
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
        if(index < lines.Count - 1)
        {
            index++;
            dialogueTextBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
