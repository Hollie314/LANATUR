using UnityEngine;

public class Levier : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;
    
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;
    [SerializeField] private Animator animator;

    public void Interact(PlayerInteractions interactions)
    {
        animator.SetBool("IsOpen", true);
        if (startsQuest)
            this.gameObject.GetComponent<Quests.StartQuestScript>().StartQuest();
            
        if (updateQuest)
            this.gameObject.GetComponent<Quests.UpdateQuest>().UpdateQuestProgress();
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}