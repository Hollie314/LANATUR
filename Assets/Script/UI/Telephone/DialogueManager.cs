using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using Quests;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("DialogueEditor")]
    [HideInInspector] public NPCConversation currentConversation;
    [HideInInspector] public List<Message> messagesToInstantiate = new List<Message>();
    
    private SpeechNode currentMessage;
    private List<SpeechNode> nextMessages = new List<SpeechNode>();
    public NPCConversation FirstConversation;
    
    [Header("Dialogues")]
    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    private Dialogue currentDialogue;
    
    void Start()
    {
        currentConversation = FirstConversation;
        currentDialogue = dialogues[0];
        ConversationManager.Instance.StartConversation(FirstConversation);
        Debug.Log($"conversation started: {ConversationManager.Instance.IsConversationActive}");
    }
    
    private void OnEnable()
    { 
        ConversationManager.OnConversationStarted += ConversationStart;
        ConversationManager.OnConversationEnded += ConversationEnd;
    }
    
    private void OnDisable()
    { 
        ConversationManager.OnConversationStarted -= ConversationStart; 
        ConversationManager.OnConversationEnded -= ConversationEnd; 
    }

    public void ConversationStart()
    {
        SpeechNode root = currentConversation.Deserialize().Root;
        nextMessages.Add(root);
        nextMessages[0].Event.AddListener(() => OnReceiveMessage(nextMessages[0]));
        Debug.Log($"Conversation Started, next message is {nextMessages[0].Text}");
    }

    public void ConversationEnd()
    {
        Debug.Log("Conversation Ended");
        nextMessages.Clear();
        currentMessage = null;
        if (! currentDialogue.quests.IsNullOrEmpty())
        {
            foreach (QuestScriptable quest in currentDialogue.quests)
            {
                StartQuestScript _startQuestScript = FindObjectOfType<StartQuestScript>();
                _startQuestScript.StartGivingQuest(quest);
            }
        }
    }
    
    public void OnReceiveMessage(SpeechNode speechNodeReceived)
    {
        currentMessage = speechNodeReceived;
        foreach (SpeechNode speechNode in nextMessages)
        {
            speechNode.Event.RemoveListener(() => OnReceiveMessage(speechNode));;
        }
        nextMessages.Clear();
        
        Debug.Log("conversation: Receive message ça marche");

        Message conv = new Message();
        conv.sender = currentMessage.Name;
        conv.message = currentMessage.Text;
        conv.messageFont = currentMessage.TMPFont;
        conv.sprite = null;
        
        SendMessage(conv);
        
        if (currentConversation.Deserialize().Root.NodeType == ConversationNode.eNodeType.Option)
        {
            Debug.Log("conversation: Receive message option");
        }

        if (!currentMessage.Connections.IsNullOrEmpty())
        {
            if (currentMessage.Connections[0] is SpeechConnection)
            {
                foreach(SpeechConnection connection in currentMessage.Connections)
                {
                    nextMessages.Add(connection.SpeechNode);
                } 
            }
            else
            {
                foreach(OptionConnection connection in currentMessage.Connections)
                {
                    OptionNode node = connection.OptionNode;
                    foreach (SpeechConnection speechConnection in node.Connections)
                    {
                        nextMessages.Add(speechConnection.SpeechNode);
                    }
                } 
            }
        }
        else
        {
            ConversationManager.Instance.EndConversation();
            Debug.Log("Conversation Ended");
        }

        foreach (SpeechNode speechNode in nextMessages)
        {
            speechNode.Event.AddListener(() => OnReceiveMessage(speechNode));;
        }
        
        Debug.Log("conversation: Receive message ça a fini");
    }
}
