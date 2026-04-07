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
    [SerializeField] private GameObject messageRempliPrefab;
    [SerializeField] private GameObject messageVidePrefab;
    [SerializeField] private GameObject conversationManagerGO;
    [SerializeField] private GameObject ScrollView;
    [SerializeField] private GameObject ContentReceived;
    [SerializeField] private GameObject ContentSent;
    
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
            
            bool foundADialogue = false;
            Dialogue dialogueFound = null;
            foreach (Dialogue dialogue in dialoguesWhenImageWithConditions)
            {
                if (dialogue.conditions.imageTag != null && message.infos.imageTag != null)
                {
                    if (dialogue.conditions.imageTag == message.infos.imageTag)
                    {
                        foundADialogue = true;
                        dialogueFound = dialogue;
                    }
                }
            }
            if (!foundADialogue)
            {
                dialogueFound = dialoguesWhenImageWithoutConditions[Random.Range(0, dialoguesWhenImageWithoutConditions.Count)];
            }
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            if(dialogueFound.conversation != null)
                dialogueManager.StartConv(dialogueFound);
            
        }
        if(message.messageFont != null)
            messageText.font = message.messageFont;
        Debug.Log("conversation: Receive message ça a fini d'instancier");
        
        Vector2 ScrollViewSize = ScrollView.GetComponent<RectTransform>().sizeDelta;
        Vector2 MessageSize = messageRempli.GetComponent<RectTransform>().sizeDelta;
        
        ScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollViewSize.x, ScrollViewSize.y + MessageSize.y + ContentReceived.GetComponent<VerticalLayoutGroup>().spacing);
    }
}
