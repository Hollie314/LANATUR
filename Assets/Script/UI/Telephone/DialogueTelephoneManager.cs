using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueTelephoneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messageRempliPrefab;
    [SerializeField] private GameObject messageVidePrefab;
    [SerializeField] private GameObject conversationManagerGO;
    [SerializeField] private GameObject ScrollView;
    [SerializeField] private GameObject ContentReceived;
    [SerializeField] private GameObject ContentSent;
    
    
    [Header("DialogueEditor")]    
    [SerializeField] private ConversationManager _conversationManager;

    [HideInInspector] public NPCConversation currentConversation;
    private SpeechNode currentMessage;
    private List<SpeechNode> nextMessages = new List<SpeechNode>();
    public NPCConversation FirstConversation;
    
    void Start()
    {
        currentConversation = FirstConversation;
        _conversationManager.StartConversation(FirstConversation);
        Debug.Log($"conversation started: {_conversationManager.IsConversationActive}");
    }
    
    private void OnEnable()
    { 
        ConversationManager.OnConversationStarted += ConversationStart;
        ConversationManager.OnConversationEnded += ConversationEnd;
        /*
        currentConversation = FirstConversation;
        _conversationManager.StartConversation(FirstConversation);
        */
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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
        conversationManagerGO.transform.GetChild(0).gameObject.SetActive(false);

        GameObject message = SendMessage(currentMessage.Name, currentMessage.Text, currentMessage.TMPFont);
        
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
            _conversationManager.EndConversation();
            Debug.Log("Conversation Ended");
        }

        foreach (SpeechNode speechNode in nextMessages)
        {
            speechNode.Event.AddListener(() => OnReceiveMessage(speechNode));;
        }
        
        Debug.Log("conversation: Receive message ça a fini");

        Vector2 ScrollViewSize = ScrollView.GetComponent<RectTransform>().sizeDelta;
        Vector2 MessageSize = message.GetComponent<RectTransform>().sizeDelta;
        
        ScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewSize.x, ScrollViewSize.y + MessageSize.y + ContentReceived.GetComponent<VerticalLayoutGroup>().spacing);
    }

    public GameObject SendMessage(string sender, string message = null, TMP_FontAsset messageFont = null, Sprite sprite = null)
    {
        GameObject messageRempli = new GameObject();
        GameObject messageVide = new GameObject();
        if (currentMessage.Name == "Noor") 
        {
            messageVide = Instantiate(messageVidePrefab, ContentReceived.transform);
            messageRempli = Instantiate(messageRempliPrefab, ContentSent.transform);
            Debug.Log("conversation: Noor");
        }
        else { 
            messageRempli = Instantiate(messageRempliPrefab, ContentReceived.transform);
            messageVide = Instantiate(messageVidePrefab, ContentSent.transform);
            Debug.Log("conversation: Else");
        }
        
        TextMeshProUGUI messageSender = messageRempli.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = messageRempli.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Sprite messageSprite = messageRempli.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        
        messageSender.text = sender;
        if (messageText != null)
            messageText.text = message;
        if (sprite != null)
        {
            messageSprite = sprite;
            
        }
        if(messageFont != null)
            messageText.font = messageFont;
        Debug.Log("conversation: Receive message ça a fini d'instancier");
        
        return messageRempli;
    }
}
