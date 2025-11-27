using UnityEngine;

public class Carnet : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le carnet");
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("carnet peut etre interargie avec");
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("carnet peut plus etre interargie avec");
    }
}
