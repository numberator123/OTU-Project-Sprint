using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;



public class Dialogue : MonoBehaviour

{
    public TextMeshProUGUI dialogueTextBox;
    public List<string> lines;
    public float textSpeed;
    public int index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void TriggerDialogue()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        lines.Clear();
        dialogueTextBox.text = string.Empty;
        lines.Add("Hi! I am an NPC, I can speak now");
        lines.Add("This is another line!");


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
