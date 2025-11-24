using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    public int Priority => 1;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la baie");
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
