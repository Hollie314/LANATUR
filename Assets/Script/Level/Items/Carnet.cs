using UnityEngine;

public class Carnet : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 2;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;
    
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le carnet");
        this.GetComponent<UpdateEntry>().UpdateEntry_Func();
        this.gameObject.SetActive(false);
        OpenUI openUI = FindObjectOfType<OpenUI>();
        openUI.OpenCarnet(interactions);
        
        if (startsQuest)
            this.gameObject.GetComponent<Quests.StartQuestScript>().StartQuest();
            
        if (updateQuest)
            this.gameObject.GetComponent<Quests.UpdateQuest>().UpdateQuestProgress();
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("carnet peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("carnet peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}
