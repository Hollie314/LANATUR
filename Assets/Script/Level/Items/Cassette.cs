using UnityEngine;

public class Cassette : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;
    
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la cassette");
        AudioSource audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
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
