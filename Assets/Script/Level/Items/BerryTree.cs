using UnityEngine;

public class BerryTree : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;
    private HoldItem holdItem;
    [SerializeField] private GameObject Baie;
    [SerializeField] private Transform BaieSpawn;
    // event interacted

    public int Priority { get; set; } = 1;
    public bool CanInteract => true;



    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la baie");
        Instantiate(Baie, BaieSpawn.position, Quaternion.identity);
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
