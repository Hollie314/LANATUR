using UnityEngine;

public class Camera_Flash : MonoBehaviour
{
    [SerializeField] private GameObject FlashLight;

    public void SetFlashActive()
    {
        FlashLight.SetActive(!FlashLight.activeSelf);
    }
}
