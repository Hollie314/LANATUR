using UnityEngine;
using DialogueEditor;

public class NPC : MonoBehaviour
{
    public NPCConversation conversation;

    void OnMouseDown()
    {
        ConversationManager.Instance.StartConversation(conversation);
    }
}