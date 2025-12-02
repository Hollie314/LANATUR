using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Camera_InputManager : MonoBehaviour
{
    [SerializeField] private Toggle ScanToggle;
    [SerializeField] private Toggle FlashToggle;
    [SerializeField] private Toggle VisionNocturneToggle;
    public static event Action<Camera_InputManager> OnZoom;
    
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
    }

    private void Start()
    {
        inputManager.Controls.UI.Scan.performed += OnScanInput;
        inputManager.Controls.UI.Flash.performed += OnFlashInput;
        inputManager.Controls.UI.VisionNocturne.performed += OnVisionNocturneInput;
        inputManager.Controls.UI.Zoom.performed += OnZoomInput;
    }

    public void OnZoomInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Zoom");
        
        OnZoom?.Invoke(this);
    }

    public void OnFlashInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Flash");
        
        FlashToggle.isOn = !FlashToggle.isOn;
    }
    
    public void OnScanInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Scan");
        
        ScanToggle.isOn = !ScanToggle.isOn;
    }
    
    public void OnVisionNocturneInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Vision nocturne");
        
        VisionNocturneToggle.isOn = !VisionNocturneToggle.isOn;
    }
}
