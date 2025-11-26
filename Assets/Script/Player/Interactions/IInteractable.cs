using UnityEngine;

public interface IInteractable
{
    int Priority { get; }
    bool CanInteract {  get; }

    void OnPlayerEnter(PlayerInteractions interactions);
    void OnPlayerExit(PlayerInteractions interactions);
    void Interact(PlayerInteractions interactions);
}
