using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities;

public class Game_Manager : MonoBehaviour
{
    public bool CameraIsActive = false;
    public bool ScanIsActive = false;
    public List<EncyclopedieEntry> EncyclopedieEntries = new List<EncyclopedieEntry>();

    void OnEnable()
    {
        Camera_Scan.OnScanActive += ChangeScanActive;
        // OpenUI.OnCameraActive += ChangeCameraActive;
    }

    private void OnDisable()
    {
        Camera_Scan.OnScanActive -= ChangeScanActive;
        // PlayerInteract.OnCameraActive -= ChangeCameraActive;
    }

    private void ChangeCameraActive() // Destroy
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
