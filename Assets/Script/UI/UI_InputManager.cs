using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InputManager : MonoBehaviour
{
    [SerializeField] private GameObject firstButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

}
