using UnityEngine;

public class BerryTree : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;
    private HoldItem holdItem;
    [SerializeField] private GameObject Baie;
    [SerializeField] private Transform BaieSpawn;
    [SerializeField] private GameObject interactionText;
    // event interacted

    public int Priority { get; set; } = 1;
    public bool CanInteract { get; set; } = true;



    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la baie");
        Instantiate(Baie, BaieSpawn.position, Quaternion.identity);
        interactionText.SetActive(false);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("Bait peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("Bait peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}
