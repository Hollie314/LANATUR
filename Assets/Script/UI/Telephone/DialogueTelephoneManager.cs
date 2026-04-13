using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using Quests;
using Sirenix.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueTelephoneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messageNoor;
    [SerializeField] private GameObject messageAutre;
    [SerializeField] private GameObject conversationManagerGO;
    [SerializeField] private GameObject ScrollView;
    [SerializeField] private GameObject Content;
    
    [Header("DialogueEditor")]    
    [HideInInspector] public NPCConversation currentConversation;
    [HideInInspector] public List<Message> messagesToInstantiate = new List<Message>();
    
    private SpeechNode currentMessage;
    private List<SpeechNode> nextMessages = new List<SpeechNode>();
    public NPCConversation FirstConversation;
    
    [Header("Dialogues")]
    [SerializeField] private List<Dialogue> dialoguesWhenImageWithConditions = new List<Dialogue>();
    [SerializeField] private List<Dialogue> dialoguesWhenImageWithoutConditions = new List<Dialogue>();
    private Dialogue currentDialogue;
    
    private void OnEnable()
    { 
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        foreach (Message message in messagesToInstantiate)
        {
            Debug.Log($"conv Send message ONENABLE {message.message}");
            SendMessage(message);
        }
        messagesToInstantiate.Clear();
    }

    private void FixedUpdate()
    {
        foreach (Message message in messagesToInstantiate)
        {
            Debug.Log($"conv Send message {message.message}");
            SendMessage(message);
        }
        messagesToInstantiate.Clear();
    }
    

    public void SendMessage(Message message)
    {
        GameObject messageSent = new GameObject();
        if (message.sender == "Noor") 
        {
            messageSent = Instantiate(messageNoor, Content.transform);
        }
        else { 
            messageSent = Instantiate(messageAutre, Content.transform);
        }
        
        TextMeshProUGUI messageSender = messageSent.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();  ////////////////////////////////////////////////////////////
        TextMeshProUGUI messageText = messageSent.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        messageSender.text = message.sender;
        if (message.message != null)
            messageText.text = message.message;
        if (message.sprite != null)
        {
            // messageSent.GetComponent<RectTransform>().sizeDelta = new Vector2(614, 400);
            // messageSent.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(614, 345);
            messageSent.transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().sprite = message.sprite;
            messageSent.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            messageSent.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            // messageSent.transform.GetChild(1).GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(614, 345);
        }
        if(message.messageFont != null)
            messageText.font = message.messageFont;
        Debug.Log("conversation: Receive message ça a fini d'instancier");
        
        /*
        Vector2 ScrollViewSize = ScrollView.GetComponent<RectTransform>().sizeDelta;
        Vector2 MessageSize = messageSent.GetComponent<RectTransform>().sizeDelta;
        
        ScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewSize.x, ScrollViewSize.y + MessageSize.y + ContentReceived.GetComponent<VerticalLayoutGroup>().spacing);
        */
    }
}
