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

    private Camera cam;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    public static event Action<PlayerInteract> OnCameraActive;
    private bool isCrouched = false;

    private bool inTriggerZoneRocher = false;
    private bool inTriggerZoneBaie = false;
    private GameObject Rocher;
    private GameObject Baie;
    private bool holdingBaie;
    public Transform BaieHolder;


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
    }

    private void Inputs()
    {
        if (action_Interact.action.WasPressedThisFrame() && !UI_Carnet.activeSelf)
        {
            if(UI_Camera.activeSelf)
            {
                UI_Camera.SetActive(false);
            }
            Interact();
        }
        if (action_UICamera.action.WasPressedThisFrame())
        {
            UI_ToCamera();
            if (holdingBaie)
            {
                if (UI_Camera.activeSelf)
                {
                    Baie.transform.parent.gameObject.SetActive(false);
                }
                else { Baie.transform.parent.gameObject.SetActive(true); }
            }
        }
        if (action_UICarnet.action.WasPressedThisFrame())
        {
            UI_ToCarnet();
            if (holdingBaie)
            {
                if (UI_Carnet.activeSelf)
                {
                    Baie.transform.parent.gameObject.SetActive(false);
                }
                else { Baie.transform.parent.gameObject.SetActive(true); }
            }
        }
    }

    private void Interact()
    {
        if (inTriggerZoneBaie)
        {
            holdingBaie = !holdingBaie;
            if (holdingBaie)
            {
                Baie.transform.parent.transform.parent = BaieHolder;
                Baie.transform.parent.transform.position = BaieHolder.transform.position;
                Baie.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
                Baie.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                Baie.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
                Debug.Log("tiens une baie");
            }
            else
            {
                Baie.transform.parent.transform.parent = null;
                Baie.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
                Baie.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                Baie.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
                Baie.transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.GetChild(0).forward * 300, ForceMode.Force);
                Debug.Log("Lache une baie");
            }
        }
        if (inTriggerZoneRocher)
        {
            Debug.Log("active un rocher");
            Rocher.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rope rock"))
        {
            Debug.Log("sur un rocher");
            inTriggerZoneRocher = true;
            Rocher = other.gameObject;
        }
        if (other.CompareTag("BaieTest"))
        {
            inTriggerZoneBaie = true;
            Baie = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rope rock"))
        {
            inTriggerZoneRocher = false;
        }
        if (other.CompareTag("BaieTest"))
        {
            inTriggerZoneBaie = false;
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
}
