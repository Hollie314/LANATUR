using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;

    private HoldItem holdItem;

    public bool IsHold { get; set; }

    [field: SerializeField] public float LifeTime { get; private set; }
    [SerializeField] private GameObject interactionText;

    public float CurrentLife { get; set; }
    // event interacted

    public int Priority { get; set; } = 3;

    public bool CanInteract { get; set; } = true;

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
        interactionText.SetActive(true);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("Baie peut etre interargie avec");
        interactionText.SetActive(false);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("Baie peut plus etre interargie avec");
        interactionText.SetActive(true);
    }
}
