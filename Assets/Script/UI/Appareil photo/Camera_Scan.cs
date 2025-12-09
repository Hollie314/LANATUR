using System;
using UnityEngine;

public class Camera_Scan : MonoBehaviour
{

    public static event Action<Camera_Scan> OnScanActive;
    public static event Action<Camera_Scan> OnScanInactive;

    public void Scan()
    {
        OnScanActive?.Invoke(this);
        Debug.Log("Scanning");
    }
}
