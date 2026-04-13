using UnityEngine;

public class BerryTree : MonoBehaviour, IInteractable
{
    private HoldItem holdItem;
    [SerializeField] private GameObject Baie;
    [SerializeField] private Transform BaieSpawn;
    [SerializeField] private GameObject interactionText;

    public int Priority { get; set; } = 1;
    public bool CanInteract { get; set; } = true;

    private void Awake()
    {
        holdItem = FindFirstObjectByType<HoldItem>();
    }

    public void Interact(PlayerInteractions interactions)
    {
        ////Debug.Log("Interaction avec la baie");

        // Spawn la baie puis la met directement en main
        GameObject baieObj = Instantiate(Baie, BaieSpawn.position, Quaternion.identity);
        Berry berry = baieObj.GetComponent<Berry>();

        if (berry != null && holdItem != null)
            holdItem.Hold(baieObj);

        interactionText.SetActive(false);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        interactionText.SetActive(false);
    }
}