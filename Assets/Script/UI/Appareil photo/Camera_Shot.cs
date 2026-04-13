using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Camera_Shot : MonoBehaviour
{
    [Header("Other UI")]
    [SerializeField] private Camera_UI Camera_UI;
    [SerializeField] ChangeEntryPhoto ChangeEntryPhoto;
    
    [Header("Photo Layers")]
    [SerializeField] private LayerMask MaskCameraVisible;
    [SerializeField] private LayerMask MaskCameraOnShot;
    [SerializeField] private LayerMask animals_LayerMask;
    
    [Header("Cap Photo")]
    [SerializeField] private float timeBetweenPictures;
    [SerializeField] private float timeBeforeClosingUI;
    private float timeSinceLastPicture;

    [Header("Raycast")]
    [HideInInspector] public List<GameObject> interestPointsVisible = new List<GameObject>();
    [SerializeField] private float distanceToSeePoint;
    
    [Header("Cursor Zoom")]
    [SerializeField] private GameObject photoCursor;
    [SerializeField] private Color colorOnNothing;
    [SerializeField] private Color colorOnTarget;
    [SerializeField] private float maxDistanceToZoom;
    [SerializeField] private float cursorSizeOnNothing;
    [SerializeField] private float cursorSizeOnTarget;
    
    [Header("Lock target")]
    [SerializeField] private GameObject lockTarget;
    
    [Header("SFX")]
    [SerializeField] private AudioSource audioSource_Photo;
    [SerializeField] private AudioClip SFX_OpenPhotoUI;
    [SerializeField] private AudioClip SFX_TakePhoto;
    [SerializeField] private AudioClip SFX_TargetLocked;

    [Header("VFX")]
    [SerializeField] private GameObject placeholderVFX;
    [SerializeField] private GameObject ShotAnim;

    // Events
    public static event Action PictureTaken;
    public static event Action<string> SpecieTakenInPhoto;

    // private
    private Texture2D screenCapture;
    private GameObject target;
    private OpenUI _openUI;
    private bool canChangeCameraUI = true;
    
    // Album
    public static Album album = new Album();
    
    private void OnEnable()
    {
        lockTarget.SetActive(false);
        
        canChangeCameraUI = true;
        _openUI = FindObjectOfType<OpenUI>();
        _openUI.canChangeCameraUI = true;
        
        ChangeEntryPhoto = FindFirstObjectByType<ChangeEntryPhoto>();
        
        audioSource_Photo.clip = SFX_OpenPhotoUI;
        audioSource_Photo.Play();
    }

    private void OnDisable()
    {
        interestPointsVisible.Clear();
        lockTarget.SetActive(false);
        if (target != null)
        {
            target = null;
        }
    }

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, DefaultFormat.HDR, TextureCreationFlags.MipChain); // Change dimensions
    }
    
    public List<GameObject> GetVisibleObjects()
    {
        List<GameObject> visibleObjects = new List<GameObject>();

        // Get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (Renderer rend in renderers)
        {
            GameObject obj = rend.gameObject;

            // Check layer
            if (((1 << obj.layer) & animals_LayerMask) == 0)
                continue;

            // Check if inside camera frustum
            if (GeometryUtility.TestPlanesAABB(planes, rend.bounds))
            {
                Debug.Log($"CAMERASHOT {obj.name} is visible part -2");
                if (obj.TryGetComponent<UpdateEntry>(out UpdateEntry animalPart) || obj.transform.parent.TryGetComponent<UpdateEntry>(out UpdateEntry updatePart))
                {
                    //how far you can check a point
                    
                    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
                    // Make a 2D vector (ignore Z)
                    Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
                    // Calculate 2D distance from center
                    float distance = Vector2.Distance(screenCenter, screenPos2D);
                    Debug.Log($"CAMERASHOT {obj.name} is visible part -1, distance: {distance}");
                    if (distance > distanceToSeePoint)
                        continue;
                    
                    Debug.Log($"CAMERASHOT {obj.name} is visible part 0");
                    if (planes.All(plane => plane.GetDistanceToPoint(obj.transform.position) >= 0))
                    {
                        Debug.Log($"CAMERASHOT {obj.name} is visible part 1");
                        Vector3 cameraPos = Camera.main.transform.position;
                        Vector3 direction = (obj.transform.position - cameraPos).normalized;

                        if (Physics.Raycast(cameraPos, direction, out RaycastHit hit))
                        {
                            Debug.Log($"CAMERASHOT {obj.name} is visible part 2, hit.name: {hit.collider.name}");
                            if (hit.collider.gameObject == obj || hit.collider.gameObject == obj.transform.parent.gameObject)
                            {
                                Debug.Log($"CAMERASHOT {obj.name} is visible part 3");
                                
                                // Fin
                                if (! obj.TryGetComponent<UpdateEntry>(out UpdateEntry onSenBranle))
                                {
                                    obj = obj.transform.parent.gameObject;
                                }
                                visibleObjects.Add(obj);
                                Debug.Log($"CAMERASHOT is visible part 4 added {obj.name}");
                            }
                        }
                    }
                }
            }
        }

        return visibleObjects;
    }

    private void Update()
    {
        Debug.Log($"Can change camera: {canChangeCameraUI}, OpenUI: {_openUI.canChangeCameraUI}");
        
        timeSinceLastPicture += Time.deltaTime;
        if (!canChangeCameraUI && timeSinceLastPicture >= timeBeforeClosingUI)
        {
            canChangeCameraUI = true;
            _openUI.canChangeCameraUI = true;
        }

        interestPointsVisible = GetVisibleObjects();
        if (!interestPointsVisible.Contains(target))
            target = null;
        CameraDetection();
        if (Input.GetMouseButtonDown(0))     // Use new input system ---------------------------------------------------------------------------------------------------
        {
            //Caper les interactions
            if (timeSinceLastPicture >= timeBetweenPictures)
            {
                timeSinceLastPicture = 0f;
                canChangeCameraUI = false;
                _openUI.canChangeCameraUI = false;
                
                StartCoroutine(TakePicture());
                PictureTaken?.Invoke();
            }
        }

        if (target != null)
        {
            if(!lockTarget.activeSelf)
                lockTarget.SetActive(true);
            // Convert object world position to screen space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            // Make a 2D vector (ignore Z)
            Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
            lockTarget.transform.position = screenPos2D;
        }
        else
        {
            if(lockTarget.activeSelf)
                lockTarget.SetActive(false);
        }

        // ZoomCursor
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float closestDistance = Mathf.Infinity;
        if (!interestPointsVisible.IsNullOrEmpty())
        {
            foreach (GameObject interestPoint in interestPointsVisible)
            {
                Debug.Log($"added list length; {interestPointsVisible.Count}");
                    Debug.Log($"added doing {interestPoint.gameObject.name}");
                    // Convert object world position to screen space
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(interestPoint.transform.position);

                    // Make a 2D vector (ignore Z)
                    Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);

                    // Calculate 2D distance from center
                    float distance = Vector2.Distance(screenCenter, screenPos2D);
                    if (distance < closestDistance)
                        closestDistance = distance;
            }
        }

        if (closestDistance >= maxDistanceToZoom)
        {
            SetCursorSizeAndColor(cursorSizeOnNothing, colorOnNothing);
            return;
        }

        float percentage = (closestDistance / maxDistanceToZoom);
        float zoom = Mathf.Lerp(cursorSizeOnTarget, cursorSizeOnNothing, percentage);
        Color color = Color.Lerp(colorOnTarget, colorOnNothing, percentage);
        Debug.Log($"closest distance: {closestDistance}, percentage: {percentage}, Zoom: {zoom}, cursorSizeOnTarget: {cursorSizeOnTarget}, cursorSizeOnNothing: {cursorSizeOnNothing}");
        SetCursorSizeAndColor(zoom, color);
    }
    
    private void SetCursorSizeAndColor(float zoom, Color color)
    {
        photoCursor.GetComponent<RectTransform>().localScale = Vector3.one * zoom;
        photoCursor.GetComponent<Image>().color = color;
    }

    private void CameraDetection()
    {
        if (interestPointsVisible.IsNullOrEmpty())
            return;
        
        RaycastHit hitInfo;
        //Debug.Log(Camera.main.farClipPlane);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hitInfo, Camera.main.farClipPlane, animals_LayerMask))
        {
            Debug.Log($"CAMERASHOT is visible part 5 {hitInfo.collider.name} {Time.fixedTime}");
                if (target != hitInfo.collider.gameObject)
                {
                    Debug.Log($"CAMERASHOT is visible part 6 {hitInfo.collider.name} {Time.fixedTime}");
                    if (interestPointsVisible.Contains(hitInfo.collider.gameObject))
                    {
                        target = hitInfo.collider.gameObject;
                        Debug.Log($"CAMERASHOT is visible part 7 {target.name} {Time.fixedTime}");
                
                        audioSource_Photo.clip = SFX_TargetLocked;
                        audioSource_Photo.Play();
                        return;
                    }
                    else if (interestPointsVisible.Contains(hitInfo.collider.gameObject.transform.parent.gameObject))
                    {
                        target = hitInfo.collider.gameObject;
                        Debug.Log($"CAMERASHOT is visible part 7 {target.name} {Time.fixedTime}");
                
                        audioSource_Photo.clip = SFX_TargetLocked;
                        audioSource_Photo.Play();
                        return;
                    }
                }
        }
    }

    private void OnTargetExitView()
    {
        target = null;
    }

    #region TakePhoto
    IEnumerator TakePicture()
    {
        yield return new WaitForEndOfFrame();
        audioSource_Photo.clip = SFX_TakePhoto;
        audioSource_Photo.Play();
        CapturePhoto();
        Camera_UI.UpdateAlbumPicture(screenCapture);
    }

    private void CapturePhoto()
    {
        // Create Texture
        Camera.main.cullingMask = MaskCameraOnShot;
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = rt;

        Camera.main.Render();

        RenderTexture.active = rt;

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, true);
        screenCapture.Apply();
        
        Camera.main.cullingMask = MaskCameraVisible;

        // Album
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
                    PhotoInfos infos = SaveSystem.SavePicture(screenCapture, target.tag, true, target.GetComponent<UpdateEntry>().EncyclopedieEntry[0], target.GetComponent<UpdateEntry>().EntryType);
                    if ( target.GetComponent<UpdateEntry>() != null)
                    {
                        target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                        ChangeEntryPhoto.AddPhotoToEntries(infos);
                    }
                    
                    SpecieTakenInPhoto?.Invoke(target.tag);
                    Debug.Log("new species");
                }
                else
                {
                    SaveSystem.SavePicture(screenCapture, target.tag, false, null, target.GetComponent<UpdateEntry>().EntryType);
                    Debug.Log("not a new species");
                    SpecieTakenInPhoto?.Invoke(target.tag);
                }
            }
            else
            {
                PhotoInfos infos = SaveSystem.SavePicture(screenCapture, target.tag, true, target.GetComponent<UpdateEntry>().EncyclopedieEntry[0], target.GetComponent<UpdateEntry>().EntryType);
                if (target.GetComponent<UpdateEntry>() != null)
                {
                    target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                    ChangeEntryPhoto.AddPhotoToEntries(infos);
                }
                
                SpecieTakenInPhoto?.Invoke(target.tag);
                Debug.Log("no photos in album");
            }
        }
        else
        {
            SaveSystem.SavePicture(screenCapture, null, false, null, PhotoInfos.ImageTypes.None);
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
    #endregion
}
