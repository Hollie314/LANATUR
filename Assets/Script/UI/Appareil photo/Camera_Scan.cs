using System;
using UnityEngine;

public class Camera_Scan : MonoBehaviour
{

    public static event Action<Camera_Scan> OnScanActive;
    public static event Action<Camera_Scan> OnScanInactive;
    private bool canScan;

    private void OnEnable()
    {
        OpenUI.OnCameraPhoto += TurnOffScan;
    }

    private void OnDisable()
    {
        OpenUI.OnCameraPhoto -= TurnOffScan;
    }

    public void Scan()
    {
        OnScanActive?.Invoke(this);
        Debug.Log("Scanning");
    }
    
    public void TurnOffScan(OpenUI openUI)
    {
        OnScanInactive?.Invoke(this);
        Debug.Log("Ending Scan");
    }
}
