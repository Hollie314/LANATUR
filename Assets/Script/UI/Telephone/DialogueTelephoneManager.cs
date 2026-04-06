using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private Sprite spritedemerd;
    
    
    [Header("DialogueEditor")]    
    [SerializeField] private ConversationManager _conversationManager;

    [HideInInspector] public NPCConversation currentConversation;
    [HideInInspector] public List<Message> messagesToInstantiate = new List<Message>();
    
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

        foreach (Message message in messagesToInstantiate)
        {
            SendMessage(message);
        }
        messagesToInstantiate.Clear();
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
            _conversationManager.EndConversation();
            Debug.Log("Conversation Ended");
        }

        foreach (SpeechNode speechNode in nextMessages)
        {
            speechNode.Event.AddListener(() => OnReceiveMessage(speechNode));;
        }
        
        Debug.Log("conversation: Receive message ça a fini");
    }

    public void SendMessage(Message message)
    {
        GameObject messageRempli = new GameObject();
        GameObject messageVide = new GameObject();
        if (message.sender == "Noor") 
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
        
        messageSender.text = message.sender;
        if (message.message != null)
            messageText.text = message.message;
        if (message.sprite != null)
        {
            messageRempli.transform.GetChild(1).GetComponent<Image>().sprite = message.sprite;
            Debug.Log("a mis sprite 1");
            messageRempli.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
            messageRempli.GetComponent<RectTransform>().sizeDelta = messageRempli.transform.GetChild(1).GetComponent<Image>().rectTransform.sizeDelta;
        }
        if(message.messageFont != null)
            messageText.font = message.messageFont;
        Debug.Log("conversation: Receive message ça a fini d'instancier");
        
        Vector2 ScrollViewSize = ScrollView.GetComponent<RectTransform>().sizeDelta;
        Vector2 MessageSize = messageRempli.GetComponent<RectTransform>().sizeDelta;
        
        ScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewSize.x, ScrollViewSize.y + MessageSize.y + ContentReceived.GetComponent<VerticalLayoutGroup>().spacing);
    }
}
