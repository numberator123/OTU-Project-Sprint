using System.Collections.Generic;
using UnityEngine;

// Read-only at runtime: changes made to this asset during Play mode can be saved into the file in the editor.
[CreateAssetMenu(fileName = "NewNPC", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;

    [Tooltip("Who this NPC is. Notes for writers.")]
    [TextArea(2, 5)]
    public string personality;

    [Tooltip("Said the first time the player talks to this NPC.")]
    public List<string> greetingLines = new List<string>();

    [Tooltip("Said on every later conversation. If empty, the greeting lines are used again.")]
    public List<string> repeatLines = new List<string>();

    [Range(0, 100)]
    public int friendliness = 50;

    public bool canTalk = true;
}
