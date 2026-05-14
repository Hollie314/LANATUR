using UnityEngine;

public class Berry : MonoBehaviour, IInteractable
{
    private Transform BaieHolder;

    private HoldItem holdItem;

    public bool IsHold { get; set; }

    [field: SerializeField] public float LifeTime { get; private set; }

    [SerializeField] private GameObject interactionText;

    public float CurrentLife { get; set; }

    public int Priority { get; set; } = 3;

    public bool CanInteract { get; set; } = true;

    private void Awake()
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
                Destroy(gameObject);
            }
        }
    }

    public void Interact(PlayerInteractions interactions)
    {
        holdItem.Hold(gameObject);

        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        if (interactionText != null)
        {
            interactionText.SetActive(true);
        }
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Sécurité supplémentaire pour éviter les références cassées
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }
}