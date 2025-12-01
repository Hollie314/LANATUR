using UnityEngine;

public interface IInteractable
{
    public int Priority { get; set; }
    bool CanInteract {  get; }

    void OnPlayerEnter(PlayerInteractions interactions);
    void OnPlayerExit(PlayerInteractions interactions);
    void Interact(PlayerInteractions interactions);
}
