using UnityEngine;

public class RockForRope : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject Corde;
    [SerializeField] private GameObject interactionText;
    
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;

    public bool CanInteract { get; set; } = true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le rocher");
        Corde.SetActive(true);
        interactionText.SetActive(false);
        
        if (startsQuest)
            this.gameObject.GetComponent<Quests.StartQuestScript>().StartQuest();
            
        if (updateQuest)
            this.gameObject.GetComponent<Quests.UpdateQuest>().UpdateQuestProgress();
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("rocher peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("rocher peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}
