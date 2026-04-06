using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    [Header("---------- Inputs ----------")]
    private static RaycastHit[] hits = new RaycastHit[16];
    public event Action<IInteractable> OnNewInteractable;
    public static event Action<PlayerInteractions> OnOpenCamera;
    public static event Action<PlayerInteractions> OnOpenCarnet;
    public static event Action<PlayerInteractions> OnOpenAlbum;
    public static event Action<PlayerInteractions> OnOpenTelephone;
    public static event Action<PlayerInteractions> OnQuitUI;
    public static event Action<PlayerInteractions> OnPause;

    [SerializeField]
    private float range;
    [SerializeField]
    private float radius;
    [SerializeField]
    private LayerMask layerMask;

    private IInteractable interactable;
    private InputManager inputManager;

    private void Awake()
    {
        inputManager = GetComponentInParent<InputManager>();
    }

    private void Start()
    {
        inputManager.Controls.OnFoot.Interact.performed += OnInteractInput;
        inputManager.Controls.OnFoot.Jump.performed += OnJumpInput;
        inputManager.Controls.OnFoot.Crouch.performed += OnCrouchInput;
        inputManager.Controls.OnFoot.OpenCamera.performed += OnOpenCameraInput;
        inputManager.Controls.OnFoot.OpenCarnet.performed += OnOpenCarnetInput;
        inputManager.Controls.OnFoot.OpenTelephone.performed += OnOpenTelephoneInput;
        inputManager.Controls.OnFoot.OpenAlbum.performed += OnOpenAlbumInput;
        inputManager.Controls.OnFoot.QuitUI.performed += OnQuitUIInput;
        inputManager.Controls.OnFoot.Pause.performed += OnPauseInput;
    }

    private void FixedUpdate()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

        int hitCount = Physics.SphereCastNonAlloc(ray, radius, hits, range, layerMask);
        IInteractable nextInteractable = null;
        for(int i = 0; i < hitCount; i++)
        {
            var hit = hits[i];
            if(hit.collider.TryGetComponent(out IInteractable component))
            {
                if(nextInteractable == null || nextInteractable.Priority < component.Priority)
                {
                    nextInteractable = component;
                }
            }
        }
        if(nextInteractable != null && nextInteractable.Priority < 0f)
        {
            nextInteractable = null;
        }

        if(nextInteractable != interactable)
        {
            if(interactable != null)
                interactable.OnPlayerExit(this);

            if(nextInteractable != null)
                nextInteractable.OnPlayerEnter(this);

            interactable = nextInteractable;
            OnNewInteractable?.Invoke(interactable);
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Interact pressed");

        if (interactable != null && interactable.CanInteract)
        {
            interactable.Interact(this);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Jump");
    }
    
    public void OnCrouchInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Crouch");
    }
    
    public void OnOpenCameraInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Opening Camera");
        
        // event OpenCamera
        OnOpenCamera?.Invoke(this);
    }
    
    public void OnOpenTelephoneInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Opening Phone");
        
        // event OpenCamera
        OnOpenTelephone?.Invoke(this);
    }
    
    public void OnOpenCarnetInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Opening Carnet");
        
        // event OpenCarnet
        OnOpenCarnet?.Invoke(this);
    }
    
    public void OnOpenAlbumInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Opening Carnet");
        
        // event OpenCarnet
        OnOpenAlbum?.Invoke(this);
    }
    
    public void OnQuitUIInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Quit UI");
        
        // event OpenCarnet
        OnQuitUI?.Invoke(this);
    }
    
    public void OnPauseInput(InputAction.CallbackContext context)
    {
        // Debug
        Debug.Log("Pause");
        
        // event OpenCarnet
        OnPause?.Invoke(this);
    }
}
