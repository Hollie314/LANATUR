using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("---------- Inputs ----------")]
    public InputActionReference action_Interact;
    public InputActionReference action_Jump;
    public InputActionReference action_Move;
    public InputActionReference action_Crouch;
    public InputActionReference action_UICamera;
    public InputActionReference action_UICarnet;

    [Header("---------- UI ----------")]
    public GameObject UI_Camera;
    public GameObject UI_Carnet;

    [Header("---------- Camera ----------")]
    public GameObject PlayerHead;
    public Transform standingTransform;
    public Transform crouchingTransform;

    private Camera cam;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    public static event Action<PlayerInteract> OnCameraActive;
    private bool isCrouched = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs(); 

        playerUI.UpdateText(string.Empty);
        //create a ray at the center of the camera, shooting outwards.
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo; //variable to store our collision information.
        
        if (Physics.Raycast(ray, out hitInfo, distance, mask)) // changer par un cube raycast !!!!!!!!!!!!!!!!!
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.OnFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }

    private void Inputs()
    {
        if (action_Interact.action.WasPressedThisFrame())
        {

        }
        if (action_Crouch.action.WasPressedThisFrame())
        {
            Crouch();
        }
        if (action_UICamera.action.WasPressedThisFrame())
        {
            UI_ToCamera();
        }
        if (action_UICarnet.action.WasPressedThisFrame())
        {
            UI_ToCarnet();
        }
    }
        
    // UI To CAMERA
    private void UI_ToCamera()
    {
        UI_Camera.SetActive(!UI_Camera.activeSelf);
        UI_Carnet.SetActive(false);
        OnCameraActive?.Invoke(this);
    }

    // UI To CARNET
    private void UI_ToCarnet()
    {
        UI_Carnet.SetActive(!UI_Carnet.activeSelf);
        UI_Camera.SetActive(false);
    }

    // Crouch
    private void Crouch()
    {
        return;
        isCrouched = !isCrouched;
        if (isCrouched)
        {
            PlayerHead.transform.position = crouchingTransform.position;
        }
        else
        {
            PlayerHead.transform.position = standingTransform.position;
        }
    }

}
