using UnityEngine;

public class PlayerLook : MonoBehaviour

{
    public Camera cam;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool clampLeftRight = false;
    
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

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

    public void ClampLeftRightRotation(bool clamp)
    {
        clampLeftRight = clamp;
    }
}
