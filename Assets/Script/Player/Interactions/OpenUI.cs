using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenUI : MonoBehaviour
{
    [Header("---------- UI ----------")]
    [SerializeField] private GameObject UI_Camera;
    [SerializeField] private GameObject UI_Carnet;
    
    // s'abonner à l'event 
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        PlayerInteractions.OnOpenCamera += OpenCamera;
        PlayerInteractions.OnOpenCarnet += OpenCarnet;
    }

    private void OnDisable()
    {
        PlayerInteractions.OnOpenCamera -= OpenCamera;
        PlayerInteractions.OnOpenCarnet -= OpenCarnet;
    }

    // UI To CAMERA
    public void OpenCamera(PlayerInteractions playerInteractions)  // Je vais faire un Prefab UI parent de toutes les UI, ça sera dedans
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        UI_Camera.SetActive(!UI_Camera.activeSelf);
        if (UI_Camera.activeSelf) 
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
        UI_Carnet.SetActive(false);
    }

    // UI To CARNET
    public void OpenCarnet(PlayerInteractions playerInteractions)
    {
        if (UI_Camera.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
        UI_Carnet.SetActive(!UI_Carnet.activeSelf);
        UI_Camera.SetActive(false);
    }
    
    public void CloseAllUI()
    {
        UI_Camera.SetActive(false);
        UI_Carnet.SetActive(false);
    }
}
