using UnityEngine;
using TMPro;

public class Digicode : MonoBehaviour, IInteractable
{
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract => true;
    
    [SerializeField] private string code;
    [SerializeField] private TextMeshProUGUI codeTMP;

    private string writtenCode;

    public void Interact(PlayerInteractions interactions)
    {
        Debug.Log("Interaction avec le digicode");
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        interactionText.SetActive(!interactionText.activeSelf);
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
    
    public void CompleteCode(string str)
    {
        writtenCode += str;
        codeTMP.text = writtenCode;
    }
    
    public void Validate()
    {
        if (writtenCode == code)
        {
            Win();
            return;
        }
        
        Lose();
    }

    private void Win()
    {
        writtenCode = "";
        codeTMP.text = writtenCode;
    }

    private void Lose()
    {
        writtenCode = "";
        codeTMP.text = writtenCode;
    }
}
