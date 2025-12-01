using UnityEngine;

public class RockForRope : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject Corde;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le rocher");
        Corde.SetActive(true);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("rocher peut etre interargie avec");
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("rocher peut plus etre interargie avec");
    }
}
