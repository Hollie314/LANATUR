using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;

    private HoldItem holdItem;
    // event interacted

    public int Priority => 1;

    public bool CanInteract => true;

    public void Awake()
    {
        holdItem = FindFirstObjectByType<HoldItem>();
    }

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la baie");
        holdItem.Hold(this.gameObject);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("Bait peut etre interargie avec");
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("Bait peut plus etre interargie avec");
    }
}
