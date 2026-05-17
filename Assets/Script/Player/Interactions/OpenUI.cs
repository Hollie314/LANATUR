using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenUI : MonoBehaviour
{
    [Header("---------- UI ----------")]
    [SerializeField] private GameObject UI_Camera;
    [SerializeField] private GameObject UI_Album;
    [SerializeField] private GameObject UI_Carnet;
    [SerializeField] private GameObject UI_Telephone;
    [SerializeField] private GameObject UI_Pause;
    
    private bool isCameraOpen = false;
    [HideInInspector] public bool canChangeCameraUI = true;
    
    public static event Action<OpenUI> OnCameraWalk;
    public static event Action<OpenUI> OnCameraPhoto;
    
    // s'abonner à l'event 
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        PlayerInteractions.OnOpenCamera += OpenCamera;
        PlayerInteractions.OnOpenCarnet += OpenCarnet;
        PlayerInteractions.OnOpenAlbum += OpenAlbum;
        PlayerInteractions.OnOpenTelephone += OpenTelephone;
        PlayerInteractions.OnQuitUI += QuitUI;
        PlayerInteractions.OnPause += Pause;
    }

    private void OnDisable()
    {
        PlayerInteractions.OnOpenCamera -= OpenCamera;
        PlayerInteractions.OnOpenCarnet -= OpenCarnet;
        PlayerInteractions.OnOpenAlbum -= OpenAlbum;
        PlayerInteractions.OnOpenTelephone -= OpenTelephone;
        PlayerInteractions.OnQuitUI -= QuitUI;
        PlayerInteractions.OnPause -= Pause;
    }

    // UI To CAMERA
    public void OpenCamera(PlayerInteractions playerInteractions)  // Je vais faire un Prefab UI parent de toutes les UI, ça sera dedans
    {
        if (!canChangeCameraUI)
            return;
        
        //Debug.Log("can change and did change");
        UI_Camera.SetActive(!UI_Camera.activeSelf);
        UI_Carnet.SetActive(false);
        UI_Album.SetActive(false);
        
        if (UI_Camera.activeSelf) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseScene(false);
            OnCameraPhoto?.Invoke(this);
            //Debug.Log("Open Camera");
            isCameraOpen = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            OnCameraWalk?.Invoke(this);
            //Debug.Log("Close Camera");
            isCameraOpen =  false;
        }
    }

    // UI To CARNET
    public void OpenCarnet(PlayerInteractions playerInteractions)
    {
        UI_Carnet.SetActive(!UI_Carnet.activeSelf);
        UI_Camera.SetActive(false);
        UI_Album.SetActive(false);
        UI_Telephone.SetActive(false);
        
        if (UI_Carnet.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PauseScene(true);
            OnCameraWalk?.Invoke(this);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            PauseScene(false);
            OnCameraWalk?.Invoke(this);
            
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
    }
    
    public void OpenTelephone(PlayerInteractions playerInteractions)
    {
        UI_Telephone.SetActive(!UI_Telephone.activeSelf);
        UI_Carnet.SetActive(false);
        UI_Camera.SetActive(false);
        UI_Album.SetActive(false);
        
        if (UI_Telephone.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PauseScene(true);
            OnCameraWalk?.Invoke(this);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            PauseScene(false);
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
        UI_Telephone.SetActive(false);
        
        if (UI_Album.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PauseScene(true);
            OnCameraWalk?.Invoke(this);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            PauseScene(false);
            OnCameraWalk?.Invoke(this);
            
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
    }

    private void OpenPause()
    {
        UI_Pause.SetActive(!UI_Pause.activeSelf);
        UI_Album.SetActive(false);
        UI_Camera.SetActive(false);
        UI_Carnet.SetActive(false);
        UI_Telephone.SetActive(false);
        
        if (UI_Pause.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            PauseScene(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            PauseScene(false);
        }
    }

    private void PauseScene(bool pause)
    {
        if (pause)
        {
            //Pause
            Time.timeScale = 0;
            //Debug.Log("Scene Paused");
        }
        else
        {
            //Unpause
            Time.timeScale = 1;
            //Debug.Log("Scene Unpaused");
        }
    }
    
    public void QuitUI(PlayerInteractions playerInteractions)
    {
        if (UI_Album.activeSelf)
        {
            UI_Album.SetActive(false);
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
        else if (UI_Carnet.activeSelf)
        {
            UI_Carnet.SetActive(false);
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
        else if (UI_Camera.activeSelf)
        {
            if (!canChangeCameraUI)
                return;
            
            UI_Camera.SetActive(false);
            OnCameraWalk?.Invoke(this);
        }
    }
    
    public void Pause(PlayerInteractions playerInteractions)
    {
        if (UI_Album.activeSelf)
        {
            UI_Album.SetActive(false);
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
        else if (UI_Carnet.activeSelf)
        {
            UI_Carnet.SetActive(false);
            if(isCameraOpen)
                OpenCamera(playerInteractions);
        }
        else if (UI_Camera.activeSelf)
        {
            if (!canChangeCameraUI)
                return;
            
            UI_Camera.SetActive(false);
            OnCameraWalk?.Invoke(this);
        }
        else
            OpenPause();
    }
}
