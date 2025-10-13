using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public bool CameraIsActive = false;
    public bool ScanIsActive = false;

    void OnEnable()
    {
        Camera_Scan.OnScanActive += ChangeScanActive;
        PlayerInteract.OnCameraActive += ChangeCameraActive;
    }

    private void OnDisable()
    {
        Camera_Scan.OnScanActive -= ChangeScanActive;
        PlayerInteract.OnCameraActive -= ChangeCameraActive;
    }

    private void ChangeCameraActive(PlayerInteract PlayerInteract)
    {
        Debug.Log("Change Camera Active");
        CameraIsActive = !CameraIsActive;
    }

    private void ChangeScanActive(Camera_Scan Camera_Scan)
    {
        Debug.Log("Change Scan Active");
        ScanIsActive = !ScanIsActive;
    }
}
