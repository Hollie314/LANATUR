using UnityEngine;

public interface IInteractable
{
    public int Priority { get; set; }
    public bool CanInteract {  get; set; }

    void OnPlayerEnter(PlayerInteractions interactions);
    void OnPlayerExit(PlayerInteractions interactions);
    void Interact(PlayerInteractions interactions);
}
