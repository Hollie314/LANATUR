using UnityEngine;

public class Rope : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;

    public void Interact(PlayerInteractions interactions)
    {
        //Debug.Log("Interaction avec la corde");
        interactionText.SetActive(false);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        //Debug.Log("corde peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        //Debug.Log("corde peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}
