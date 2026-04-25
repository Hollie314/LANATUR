using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour

{
    [SerializeField] private CinemachineStateDrivenCamera cam;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool clampLeftRight = false;
    
    [SerializeField] private Slider SensitivitySlider; 
    public float xSensitivity = 3;
    public float ySensitivity = 3f;

    public void ProcessLook(Vector2 input)
    {
        Cursor.lockState = CursorLockMode.Confined;

        float mouseX = input.x;
        float mouseY = input.y;
        //calculate camera rotation for looking up and down
        yRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        yRotation = Mathf.Clamp (yRotation, -80f, 80f);

        if(clampLeftRight )
        {
            //calculate camera rotation for looking Left and right
            xRotation += (mouseX * Time.deltaTime) * xSensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            //apply this to our camera transform.
            cam.transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
        }
        else
        {
            cam.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
            transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
        }
    }

    public void OnValueChanged()
    {
        if (SensitivitySlider != null)
        {
            xSensitivity = SensitivitySlider.value;
            ySensitivity = SensitivitySlider.value;   
        }
    }

    public void ClampLeftRightRotation(bool clamp)
    {
        clampLeftRight = clamp;
    }
}
