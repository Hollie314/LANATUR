using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities;

public class Game_Manager : MonoBehaviour
{
    public bool CameraIsActive { get; private set; } = false;
    public bool ScanIsActive { get; private set; } = false;
    
    public Vector3 LastPositionSaved { get; set; } = new Vector3();
    public List<EncyclopedieEntry> EncyclopedieEntries = new List<EncyclopedieEntry>();

    void OnEnable()
    {
        Camera_Scan.OnScanActive += ChangeScanActive;
        OpenUI.OnCameraPhoto += ChangeCameraActive;
    }

    private void OnDisable()
    {
        Camera_Scan.OnScanActive -= ChangeScanActive;
        OpenUI.OnCameraPhoto -= ChangeCameraActive;
        ResetEntries();
    }

    private void ChangeCameraActive(OpenUI openUI) // Destroy
    {
        Debug.Log("Change Camera Active");
        CameraIsActive = !CameraIsActive;
    }

    private void ChangeScanActive(Camera_Scan Camera_Scan)
    {
        Debug.Log("Change Scan Active");
        ScanIsActive = !ScanIsActive;
    }

    public void ResetEntries()
    {
        foreach (EncyclopedieEntry entry in EncyclopedieEntries)
        {
            entry.Anecdote = null;
            entry.Caracteristique = null;
            entry.Dessin = null;
            entry.DessinMignon = null;
            entry.NoteDeRen = null;
            entry.Photo = null;
            entry.UpdatesDone.Clear();
        }
    }
}
