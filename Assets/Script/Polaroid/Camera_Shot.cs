using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Camera_Shot : MonoBehaviour
{
    [Header("Photo Taker")]
    // public
    [SerializeField] private Camera_UI Camera_UI;
    [SerializeField] private GameManager GameManager;
    public LayerMask animals_LayerMask;
    public static event Action PictureTaken;
    
    // private
    private Texture2D screenCapture;
    private GameObject target;

    [Header("SFX")]
    // public
    public AudioSource placeholderSFX;

    // private

    [Header("VFX")]
    // public
    public GameObject placeholderVFX;

    // private

    private void OnEnable() // new
    {
        AnimalPart.ExitView += OnTargetExitView;
    }

    private void OnDisable() // new
    {
        AnimalPart.ExitView -= OnTargetExitView;
    }

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false); // Change dimensions
    }


    private void Update()
    {
        CameraDetection();
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakePicture());
            PictureTaken?.Invoke();
        }
    }

    private void CameraDetection()
    {
        RaycastHit hitInfo;
        Debug.Log(Camera.main.farClipPlane);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hitInfo, Camera.main.farClipPlane, animals_LayerMask))
        {
            {
                target = hitInfo.collider.gameObject;
                target.GetComponent<AnimalPart>().BecomeTarget();
            }
        }

        // Debug
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * Camera.main.farClipPlane, Color.red);
        if (target != null)
        {
            Debug.Log("Targets " + target.name);
        }
        else { Debug.Log("No target"); }
        return;
    }

    private void OnTargetExitView()
    {
        target = null;
    }

    IEnumerator TakePicture()
    {
        yield return new WaitForEndOfFrame();
        CapturePhoto();
        AddPhotoInformations();
        SaveToAlbum();
        Camera_UI.UpdateAlbumPicture(screenCapture);
    }

    private void CapturePhoto()
    {
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = rt;

        Camera.main.Render();

        RenderTexture.active = rt;

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        if (target != null)
        {
            SaveSystem.SavePicture(screenCapture, target.tag); //i think it's the tag the problem
        }
        else
        {
            SaveSystem.SavePicture(screenCapture, null); //i think it's the tag the problem
        }



        // Reset all
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }

    private void AddPhotoInformations()
    {

    }

    private void SaveToAlbum()
    {
        
    }

}
