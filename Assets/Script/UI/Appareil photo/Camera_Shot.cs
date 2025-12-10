using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Camera_Shot : MonoBehaviour
{
    [Header("Photo Taker")]
    // public
    [SerializeField] private Camera_UI Camera_UI;
    [SerializeField] ChangeEntryPhoto ChangeEntryPhoto;
    [SerializeField] private GameObject ShotAnim;
    public LayerMask animals_LayerMask;
    public static event Action PictureTaken;
    public static Album album = new Album();

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
    
    [Header("Raycast")]
    public List<AnimalPart> animalsOnScreen = new List<AnimalPart>();

    [SerializeField] private float maxZoomIn;
    [SerializeField] private float maxZoomOut;
    // private

    private void OnEnable() // new
    {
        AnimalPart.ExitView += OnTargetExitView;
        ChangeEntryPhoto = FindFirstObjectByType<ChangeEntryPhoto>();
        AnimalPart[]  animals = FindObjectsOfType<AnimalPart>();
        foreach (AnimalPart animalPart in animals)
        {
            animalPart.CameraShot = this;
        }
    }

    private void OnDisable() // new
    {
        AnimalPart.ExitView -= OnTargetExitView;
        animalsOnScreen.Clear();
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

        if (target == null)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            float maxDistance = 0;
            foreach (AnimalPart animalPart in animalsOnScreen)
            {
                // Convert object world position to screen space
                Vector3 screenPos = Camera.main.WorldToScreenPoint(animalPart.transform.position);

                // Make a 2D vector (ignore Z)
                Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);

                // Calculate 2D distance from center
                float distance = Vector2.Distance(screenCenter, screenPos2D);
                if (distance > maxDistance)
                    maxDistance = distance;
            }

            if (maxDistance == 0)
            {
                ZoomCursor(maxZoomOut);
                return;
            }

            float percentage = (Screen.width / 2) / maxDistance;
            float zoom = maxZoomOut + (maxZoomIn - maxZoomOut) * (percentage);
            ZoomCursor(maxZoomOut);
        }
    }

    private void ZoomCursor(float zoom)
    {
        transform.parent.transform.localScale = Vector3.one * zoom;
    }

    private void CameraDetection()
    {
        RaycastHit hitInfo;
        //Debug.Log(Camera.main.farClipPlane);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hitInfo, Camera.main.farClipPlane, animals_LayerMask))
        {
            {
                target = hitInfo.collider.gameObject;
                Debug.Log($"{target.name} {Time.fixedTime}");
                target.GetComponent<AnimalPart>().BecomeTarget();
                
                ZoomCursor(maxZoomIn);
            }
        }
        /*
        // Debug
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * Camera.main.farClipPlane, Color.red);
        if (target != null)
        {
            Debug.Log("Targets " + target.name);
        }
        else { Debug.Log("No target"); }
        */
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
            album = Album.Load();
            if (!album.photoInfos.IsNullOrEmpty())
            {
                bool isNewSpecies = true;
                foreach (var info in album.photoInfos)
                {
                    if(info.imageTag == target.tag)
                    {
                        isNewSpecies = false;
                        
                    }
                }
                if (isNewSpecies)
                {
                    PhotoInfos infos = SaveSystem.SavePicture(screenCapture, target.tag, true, target.GetComponent<UpdateEntry>().EncyclopedieEntry[0]);
                    if ( target.GetComponent<UpdateEntry>() != null)
                    {
                        target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                        ChangeEntryPhoto.AddPhotoToEntries(infos);
                    }

                    Debug.Log("new species");
                }
                else
                {
                    SaveSystem.SavePicture(screenCapture, target.tag, false, null);
                    Debug.Log("not a new species");
                }
            }
            else
            {
                PhotoInfos infos = SaveSystem.SavePicture(screenCapture, target.tag, true, target.GetComponent<UpdateEntry>().EncyclopedieEntry[0]);
                if (target.GetComponent<UpdateEntry>() != null)
                {
                    target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                    ChangeEntryPhoto.AddPhotoToEntries(infos);
                }

                Debug.Log("no photos in album");
            }
        }
        else
        {
            SaveSystem.SavePicture(screenCapture, null, false, null);
            Debug.Log("no target");
        }
        
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        ShotAnim.GetComponent<Image>().sprite = photoSprite;
        ShotAnim.GetComponent<Animator>().Play("Photo_Flash");



        // Reset all
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
    }
}
