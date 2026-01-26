using UnityEngine;

public class LetterDocument : MonoBehaviour, IInteractable
{
    [Header("Letter Data")]
    [SerializeField] private string letterID; // ID unique ou nom de la lettre
    [TextArea]
    [SerializeField] private string letterContent;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionText;

    public int Priority { get; set; } = 1;
    public bool CanInteract { get; set; } = true;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Lettre ramassée : " + letterID);

        // 👉 Exemple : ajout à un gestionnaire de documents
        DocumentManager.Instance.AddLetter(letterID, letterContent);

        interactionText.SetActive(false);

        // Supprime la lettre de la scène
        Destroy(gameObject);
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("Lettre peut être interagie");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("Lettre ne peut plus être interagie");
        interactionText.SetActive(false);
    }
}