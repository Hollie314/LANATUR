using UnityEngine;
using UnityEngine.UI;

public class Camera_UI : MonoBehaviour
{
    [Header("UI")]
    // public

    // private
    [SerializeField] private Image photoDisplayArea;

    [Header("SFX")]
    // public
    public AudioSource placeholderSFX;

    // private

    [Header("VFX")]
    // public
    public GameObject placeholderVFX;

    // private


    public void UpdateAlbumPicture(Texture2D screenCapture)
    {
        Sprite photoSprite = Sprite.Create(screenCapture, new Rect(0.0f, 0.0f, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photoDisplayArea.sprite = photoSprite;
    }
}
