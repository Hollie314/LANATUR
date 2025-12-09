using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenUI : MonoBehaviour
{
    [Header("---------- UI ----------")]
    [SerializeField] private GameObject UI_Camera;
    [SerializeField] private GameObject UI_Album;
    [SerializeField] private GameObject UI_Carnet;
    
    private bool isCameraOpen = false;
    
    public static event Action<OpenUI> OnCameraWalk;
    public static event Action<OpenUI> OnCameraPhoto;
    
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
        PlayerInteractions.OnOpenAlbum += OpenAlbum;
    }

    private void OnDisable()
    {
        PlayerInteractions.OnOpenCamera -= OpenCamera;
        PlayerInteractions.OnOpenCarnet -= OpenCarnet;
        PlayerInteractions.OnOpenAlbum -= OpenAlbum;
    }

    // UI To CAMERA
    public void OpenCamera(PlayerInteractions playerInteractions)  // Je vais faire un Prefab UI parent de toutes les UI, ça sera dedans
    {
        UI_Camera.SetActive(!UI_Camera.activeSelf);
        UI_Carnet.SetActive(false);
        UI_Album.SetActive(false);
        
        if (UI_Camera.activeSelf) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnCameraPhoto?.Invoke(this);
            Debug.Log("Open Camera");
            isCameraOpen = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            OnCameraWalk?.Invoke(this);
            Debug.Log("Close Camera");
            isCameraOpen =  false;
        }
    }

    // UI To CARNET
    public void OpenCarnet(PlayerInteractions playerInteractions)
    {
        UI_Carnet.SetActive(!UI_Carnet.activeSelf);
        UI_Camera.SetActive(false);
        UI_Album.SetActive(false);
        
        if (UI_Carnet.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            OnCameraWalk?.Invoke(this);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            OnCameraWalk?.Invoke(this);
            
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
    }
    
    // UI To ALBUM
    public void OpenAlbum(PlayerInteractions playerInteractions)
    {
        UI_Album.SetActive(!UI_Album.activeSelf);
        UI_Camera.SetActive(false);
        UI_Carnet.SetActive(false);
        
        if (UI_Album.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            OnCameraWalk?.Invoke(this);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            OnCameraWalk?.Invoke(this);
            
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
    }
    
    public void CloseAllUI()
    {
        UI_Camera.SetActive(false);
        UI_Carnet.SetActive(false);
        UI_Album.SetActive(false);
        OnCameraWalk?.Invoke(this);
    }
}
