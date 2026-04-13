using System;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Digicode : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;

    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;
    
    public static event Action<Digicode> OnCodeEntered;

    [Header("Code")]
    [SerializeField] private string code;
    [SerializeField] private TextMeshProUGUI codeTMP;

    [Header("Quêtes")]
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;

    [Header("Events")]
    [SerializeField] private UnityEvent onCorrectCode;

    private string writtenCode;

    public void Interact(PlayerInteractions interactions)
    {
        //Debug.Log("Interaction avec le digicode");
        interactionText.SetActive(!interactionText.activeSelf);

        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        if (!CanInteract)
            return;

        //Debug.Log("cassette peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        if (!CanInteract)
            return;

        //Debug.Log("cassette peut plus etre interargie avec");
        interactionText.SetActive(false);
    }

    public void CompleteCode(string str)
    {
        writtenCode += str;
        codeTMP.text = writtenCode;
    }

    public void Validate()
    {
        if (writtenCode == code)
        {
            Win();
            return;
        }

        Lose();
    }

    private void Win()
    {
        writtenCode = "";
        codeTMP.text = writtenCode;

        transform.GetChild(0).gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Time.timeScale = 1;

        CanInteract = false;
        interactionText.SetActive(false);

        
        OnCodeEntered?.Invoke(this);   
        onCorrectCode?.Invoke();       

      
        if (startsQuest)
            GetComponent<Quests.StartQuestScript>()?.StartQuest();

        if (updateQuest)
            GetComponent<Quests.UpdateQuest>()?.UpdateQuestProgress();
    }

    private void Lose()
    {
        writtenCode = "";
        codeTMP.text = writtenCode;
    }
}
