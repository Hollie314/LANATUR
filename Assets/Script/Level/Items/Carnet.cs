using UnityEngine;

public class Carnet : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 2;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le carnet");
        this.GetComponent<UpdateEntry>().UpdateEntry_Func();
        this.gameObject.SetActive(false);
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
