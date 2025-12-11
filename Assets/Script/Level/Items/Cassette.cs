using UnityEngine;

public class Cassette : MonoBehaviour
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract => true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec la cassette");
        this.GetComponent<UpdateEntry>().UpdateEntry_Func();
        Destroy(this.gameObject);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}
