using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InputManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private GameObject firstButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
