using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhotoFrame : MonoBehaviour
{
    public Toggle _toggle;
    public PhotoInfos photoInfos;
    [SerializeField] private Color toggleBackgroundOff,  toggleBackgroundOn;
    
    private AlbumUI albumUI;

    public bool canBeSelected = false;

    private void Start()
    {
        albumUI = FindObjectOfType<AlbumUI>();
        _toggle.gameObject.transform.GetChild(0).GetComponent<Image>().color = toggleBackgroundOff;
    }
    
    public void OnClicked()
    {
        // Toggle Selection Multiple
        if (canBeSelected)
        {
            _toggle.isOn = !_toggle.isOn;
            if (_toggle.isOn)
            {
                albumUI.ListSelectedPhotos.Add(photoInfos);
                _toggle.gameObject.transform.GetChild(0).GetComponent<Image>().color = toggleBackgroundOn;
            }
            else
            {
                albumUI.ListSelectedPhotos.Remove(photoInfos);
                _toggle.gameObject.transform.GetChild(0).GetComponent<Image>().color = toggleBackgroundOff;
            }
        }
        // Show Photo infos
        else
        {
            albumUI.ShowPhoto(GetComponent<Image>().sprite, photoInfos);
            albumUI.ListSelectedPhotos.Clear();
            albumUI.ListSelectedPhotos.Add(photoInfos);
        }
    }
}
