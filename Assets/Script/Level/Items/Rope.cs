using UnityEngine;

public class Rope : MonoBehaviour, IInteractable
{
    public int Priority => 1;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la corde");
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("corde peut etre interargie avec");
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("corde peut plus etre interargie avec");
    }
}
