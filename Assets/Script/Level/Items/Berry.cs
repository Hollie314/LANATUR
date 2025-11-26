using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;

    private HoldItem holdItem;

    bool isHold { get; set; }

    [SerializeField] private float LifeTime;
    // event interacted

    public int Priority => 2;

    public bool CanInteract => true;

    public void Awake()
    {
        holdItem = FindFirstObjectByType<HoldItem>();
    }

    private void FixedUpdate()
    {
        // if()
        LifeTime -= Time.deltaTime;

    }

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la baie");
        holdItem.Hold(this.gameObject);
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
