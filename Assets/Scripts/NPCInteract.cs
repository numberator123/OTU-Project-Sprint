using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using System.Collections;

public class NPCInteract : MonoBehaviour
{
    bool playerInRange = false;

    public TMP_Text interactText;
    private string backendURL = "http://localhost:5000/api/message";
    public Dialogue dialogueScript;

    void Start()
    {
        // Subscribe to the Enter-key submit event from Dialogue.cs
        dialogueScript.onPlayerSubmit += SendMessageToBackend;
    }

    void SendMessageToBackend(string playerInput)
    {
        Debug.Log("Player typed: " + playerInput);
        StartCoroutine(PostMessage(playerInput));
    }

    IEnumerator PostMessage(string message)
    {
        string json = JsonUtility.ToJson(new MessageRequest(message));

        using (UnityWebRequest request = new UnityWebRequest(backendURL, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Backend response: " + request.downloadHandler.text);
                // Show the AI's reply in the dialogue box
                dialogueScript.DisplayNPCReply(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Backend error: " + request.error);
                dialogueScript.DisplayNPCReply("Sorry, I couldn't connect to my brain.");
            }
        }
    }

    void Update()
    {
        // Only trigger dialogue with 'E' if the dialogue box isn't already open
        if (playerInRange && !dialogueScript.gameObject.activeInHierarchy && Keyboard.current.eKey.wasPressedThisFrame)
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

    [System.Serializable]
    public class MessageRequest
    {
        public string message;
        public MessageRequest(string message) { this.message = message; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactText.text = "E";
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactText.text = "";
            playerInRange = false;
        }
    }
}
