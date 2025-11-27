using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;

    private HoldItem holdItem;

    public bool IsHold { get; set; }

    [field: SerializeField] public float LifeTime { get; private set; }

    public float CurrentLife { get; set; }
    // event interacted

    public int Priority { get; set; } = 2;

    public bool CanInteract => true;

    public void Awake()
    {
        holdItem = FindFirstObjectByType<HoldItem>();
        CurrentLife = LifeTime;
    }

    private void FixedUpdate()
    {
        if (!IsHold)
        {
            CurrentLife -= Time.deltaTime;
            if (CurrentLife <= 0)
            {
                Destroy(this.gameObject);
            }
        }

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
