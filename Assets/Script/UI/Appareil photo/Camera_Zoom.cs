using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Camera_Zoom : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [Range(1.5f,5f)] [SerializeField] private float MaxZoom;
    [SerializeField] private Slider ZoomSlider;
    [SerializeField] private GameObject Zoom;
    private float baseFieldOfView;
    
    // hide zoom
    private float timerOnInactive = 0f;
    private float timerFading = 0f;
    private bool ZoomIsShown = true;
    [Range(1.5f, 5f)] [SerializeField] private float ShowTimeOnInactive;
    [Range(0f, 3f)] [SerializeField] private float FadingTime;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ZoomSlider.minValue = 1; ZoomSlider.maxValue = MaxZoom;
        baseFieldOfView = camera.fieldOfView;
        // Camera =  GetComponent<CinemachineCamera>();
        
        ZoomSlider.onValueChanged.AddListener((v) =>
        {
            ShowZoom();
            camera.fieldOfView = baseFieldOfView / v;
        });
    }

    void OnEnable()
    {
        RestartZoom();
    }

    public void RestartZoom()
    {
        ZoomSlider.value = 1;
        ShowZoom();
    }

    // Update is called once per frame
    private void ShowZoom()
    {
        Zoom.SetActive(true);
        ZoomIsShown = true;
        timerOnInactive = 0f;
    }

    private void HideZoom()
    {
        timerFading += Time.deltaTime;
        
        // effacer le zoom
        Zoom.SetActive(false);
        ZoomIsShown = false;
        timerFading = 0f;
        // remettre l'opacité
    }

    private void Update()
    {
        if (ZoomIsShown)
        {
            timerOnInactive += Time.deltaTime;
            if (timerOnInactive >= ShowTimeOnInactive)
            {
                HideZoom();
            }
        }
    }
}
