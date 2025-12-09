using UnityEngine;

public class Camera_Flash : MonoBehaviour
{
    [SerializeField] private GameObject FlashLight;

    private void OnEnable()
    {
        OpenUI.OnCameraPhoto += TurnOffFlash;
    }

    private void OnDisable()
    {
        OpenUI.OnCameraPhoto -= TurnOffFlash;
    }

    public void SetFlashActive()
    {
        FlashLight.SetActive(!FlashLight.activeSelf);
    }
    
    public void TurnOffFlash(OpenUI openUI)
    {
        FlashLight.SetActive(false);
    }
}
