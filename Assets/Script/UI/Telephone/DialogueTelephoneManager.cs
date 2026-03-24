using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using TMPro;

public class DialogueTelephoneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject conversationManagerGO;
    [SerializeField] private GameObject Content;
    
    
    [Header("DialogueEditor")]    
    [SerializeField] private ConversationManager _conversationManager;

    [HideInInspector] public NPCConversation currentConversation;
    private SpeechNode currentMessage;
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
        currentMessage = currentConversation.Deserialize().Root;
        currentMessage.Event.AddListener(OnReceiveMessage);
        Debug.Log("Conversation Started");
    }

    public void ConversationEnd()
    {
        Debug.Log("Conversation Ended");
    }
    
    public void OnReceiveMessage()
    {
        Debug.Log("conversation: Receive message ça marche");
        conversationManagerGO.transform.GetChild(0).gameObject.SetActive(false);
        GameObject message = Instantiate(messagePrefab, Content.transform);
        // message.transform.SetSiblingIndex(0);
        TextMeshProUGUI messageSender = message.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = message.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        messageSender.text = currentMessage.Name;
        messageText.text = currentMessage.Text;
        // messageText.color = Color.red;
        messageText.font = currentMessage.TMPFont;
        Debug.Log("conversation: Receive message ça a fini d'instancier");
        
        if (currentConversation.Deserialize().Root.NodeType == ConversationNode.eNodeType.Option)
        {
            Debug.Log("conversation: Receive message option");
        }
        currentMessage.Event.RemoveListener(OnReceiveMessage);

        if (currentMessage.Connections[0] is SpeechConnection)
        {
            foreach(SpeechConnection connection in currentMessage.Connections)
            {
                currentMessage = connection.SpeechNode;
            } 
        }
        else
        {
            foreach(Connection connection in currentMessage.Connections)
            {
            
            } 
        }
        
        currentMessage.Event.AddListener(OnReceiveMessage);
        Debug.Log("conversation: Receive message ça a fini");
    }
}
