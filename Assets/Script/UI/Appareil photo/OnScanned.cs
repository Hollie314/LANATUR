using System.Collections;
using UnityEngine;

public class OnScanned : MonoBehaviour
{
    Game_Manager gameManager;
    public GameObject Scans;

    void OnEnable()
    {
        Camera_Scan.OnScanActive += CheckScan;
        // Camera_Scan.OnScanInactive += StopScan;
    }

    private void OnDisable()
    {
        Camera_Scan.OnScanActive -= CheckScan;
        // Camera_Scan.OnScanInactive -= StopScan;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<Game_Manager>();
    }

    private void CheckScan(Camera_Scan Camera_Scan)
    {
        StartCoroutine(CheckScanCoroutine());
    }
    
    private void StopScan(Camera_Scan Camera_Scan)
    {
        Scans.SetActive(false);
    }

    IEnumerator CheckScanCoroutine()
    {

        yield return 0;

        Debug.Log("CheckScan");
        Scans.SetActive(!Scans.activeSelf);
        /*
        if (gameManager.CameraIsActive && gameManager.ScanIsActive)
        {
            Scans.SetActive(true);
        }
        else
        {
            Scans.SetActive(false);
        }
        */
    }

    private void Update()
    {
        if(Scans.activeSelf)
            Scans.transform.LookAt(Camera.main.transform);
    }
}
