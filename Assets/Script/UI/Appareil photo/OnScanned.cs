using System.Collections;
using UnityEngine;

public class OnScanned : MonoBehaviour
{
    Game_Manager gameManager;
    public GameObject Scans;

    void OnEnable()
    {
        Camera_Scan.OnScanActive += CheckScan1;
        // PlayerInteract.OnCameraActive += CheckScan2;
    }

    private void OnDisable()
    {
        Camera_Scan.OnScanActive -= CheckScan1;
        // PlayerInteract.OnCameraActive -= CheckScan2;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<Game_Manager>();
    }

    private void CheckScan1(Camera_Scan Camera_Scan)
    {
        StartCoroutine(CheckScanCoroutine());
    }
    
    private void CheckScan2() // Destroy
    {
        StartCoroutine(CheckScanCoroutine());
    }

    IEnumerator CheckScanCoroutine()
    {

        yield return 0;

        Debug.Log("CheckScan");
        if (gameManager.CameraIsActive && gameManager.ScanIsActive)
        {
            Scans.SetActive(true);
        }
        else
        {
            Scans.SetActive(false);
        }
    }
}
