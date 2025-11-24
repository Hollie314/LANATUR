using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    private static RaycastHit[] hits = new RaycastHit[16];
    public event Action<IInteractable> OnNewInteractable;

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
        if (interactable != null && interactable.CanInteract)
        {
            interactable.Interact(this);
        }
    }
}
