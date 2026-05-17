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
    [SerializeField] private UI_Notifications _uiNotifications;
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

    [SerializeField] private float TimeToCheckVisible = 0.3f;
    private float timeSinceLastCheckVisible = 0f;
    
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
        screenCapture = new Texture2D(Screen.width, Screen.height, DefaultFormat.HDR, TextureCreationFlags.None);
    }
    
    public List<GameObject> GetVisibleObjects()
    {
        List<GameObject> visibleObjects = new List<GameObject>();

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
                // FIX NULL PARENT
                if (
                    obj.TryGetComponent<UpdateEntry>(out UpdateEntry animalPart) ||
                    (
                        obj.transform.parent != null &&
                        obj.transform.parent.TryGetComponent<UpdateEntry>(out UpdateEntry updatePart)
                    )
                )
                {
                    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);

                    Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);

                    float distance = Vector2.Distance(screenCenter, screenPos2D);

                    if (distance > distanceToSeePoint)
                        continue;
                    
                    if (planes.All(plane => plane.GetDistanceToPoint(obj.transform.position) >= 0))
                    {
                        Vector3 cameraPos = Camera.main.transform.position;
                        Vector3 direction = (obj.transform.position - cameraPos).normalized;

                        if (Physics.Raycast(cameraPos, direction, out RaycastHit hit))
                        {
                            // FIX NULL PARENT
                            if (
                                hit.collider.gameObject == obj ||
                                (
                                    obj.transform.parent != null &&
                                    hit.collider.gameObject == obj.transform.parent.gameObject
                                )
                            )
                            {
                                if (!obj.TryGetComponent<UpdateEntry>(out UpdateEntry onSenBranle))
                                {
                                    // FIX NULL PARENT
                                    if (obj.transform.parent != null)
                                    {
                                        obj = obj.transform.parent.gameObject;
                                    }
                                }

                                visibleObjects.Add(obj);
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
        timeSinceLastPicture += Time.deltaTime;

        if (!canChangeCameraUI && timeSinceLastPicture >= timeBeforeClosingUI)
        {
            canChangeCameraUI = true;
            _openUI.canChangeCameraUI = true;
        }

        timeSinceLastCheckVisible += Time.deltaTime;

        if (timeSinceLastCheckVisible >= TimeToCheckVisible)
        {
            timeSinceLastCheckVisible = 0f;
            interestPointsVisible = GetVisibleObjects();
        }
        
        if (!interestPointsVisible.Contains(target))
            target = null;

        CameraDetection();

        if (Input.GetMouseButtonDown(0))
        {
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

            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

            Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);

            lockTarget.transform.position = screenPos2D;
        }
        else
        {
            if(lockTarget.activeSelf)
                lockTarget.SetActive(false);
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float closestDistance = Mathf.Infinity;

        if (!interestPointsVisible.IsNullOrEmpty())
        {
            foreach (GameObject interestPoint in interestPointsVisible)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(interestPoint.transform.position);

                Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);

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

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hitInfo, Camera.main.farClipPlane, animals_LayerMask))
        {
            if (target != hitInfo.collider.gameObject)
            {
                if (interestPointsVisible.Contains(hitInfo.collider.gameObject))
                {
                    target = hitInfo.collider.gameObject;
                
                    audioSource_Photo.clip = SFX_TargetLocked;
                    audioSource_Photo.Play();
                    return;
                }

                // FIX NULL PARENT
                else if (
                    hitInfo.collider.transform.parent != null &&
                    interestPointsVisible.Contains(hitInfo.collider.transform.parent.gameObject)
                )
                {
                    target = hitInfo.collider.gameObject;
                
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
        _uiNotifications.ChangeAlbum(true);
        
        Camera.main.cullingMask = MaskCameraOnShot;

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24, DefaultFormat.HDR);

        Camera.main.targetTexture = rt;

        Camera.main.Render();

        RenderTexture.active = rt;

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, true);
        screenCapture.Apply();
        
        Camera.main.cullingMask = MaskCameraVisible;

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
                    PhotoInfos infos = SaveSystem.SavePicture(
                        screenCapture,
                        target.tag,
                        true,
                        target.GetComponent<UpdateEntry>().EncyclopedieEntry[0],
                        target.GetComponent<UpdateEntry>().EntryType
                    );

                    if (target.GetComponent<UpdateEntry>() != null)
                    {
                        target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                        ChangeEntryPhoto.AddPhotoToEntries(infos);
                    }
                    
                    _uiNotifications.ChangeCarnet(true);
                    SpecieTakenInPhoto?.Invoke(target.tag);
                }
                else
                {
                    SaveSystem.SavePicture(
                        screenCapture,
                        target.tag,
                        false,
                        null,
                        target.GetComponent<UpdateEntry>().EntryType
                    );

                    SpecieTakenInPhoto?.Invoke(target.tag);
                }
            }
            else
            {
                PhotoInfos infos = SaveSystem.SavePicture(
                    screenCapture,
                    target.tag,
                    true,
                    target.GetComponent<UpdateEntry>().EncyclopedieEntry[0],
                    target.GetComponent<UpdateEntry>().EntryType
                );

                if (target.GetComponent<UpdateEntry>() != null)
                {
                    target.GetComponent<UpdateEntry>().UpdateEntry_Func();
                    ChangeEntryPhoto.AddPhotoToEntries(infos);
                }
                
                SpecieTakenInPhoto?.Invoke(target.tag);
            }
        }
        else
        {
            SaveSystem.SavePicture(
                screenCapture,
                null,
                false,
                null,
                PhotoInfos.ImageTypes.None
            );
        }
        
        Sprite photoSprite = Sprite.Create(
            screenCapture,
            new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );

        ShotAnim.GetComponent<Image>().sprite = photoSprite;
        ShotAnim.GetComponent<Animator>().Play("Photo_Flash");

        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        Destroy(rt);
    }

    #endregion
}