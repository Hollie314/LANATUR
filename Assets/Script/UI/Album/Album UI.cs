using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumUI : SerializedMonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private ChangeEntryPhoto changeEntryPhoto;
    [SerializeField] private SortAlbum sortAlbum;

    [Header("Canvas / Panels")]
    [SerializeField] private GameObject panelAlbum, panelPhoto;
    
    [Header("Buttons")]
    [SerializeField] private GameObject PanelPhoto_Photo;
    [SerializeField] private GameObject PanelPhoto_ButtonDelete;
    [SerializeField] private GameObject PanelPhoto_ButtonReplace;
    [SerializeField] private GameObject SelectionMultipleOptions;

    private GameObject PhotoZoomedOn;
    [HideInInspector] public List<PhotoInfos> ListSelectedPhotos = new List<PhotoInfos>();
    private bool selectionMultipleOn = false;

    private void OnDisable()
    {
        if (selectionMultipleOn)
        {
            SelectionMultiple();
        }
        panelAlbum.SetActive(true);
        panelPhoto.SetActive(false);
    }

    public void QuitAlbum()
    {
        OpenUI openUI = FindFirstObjectByType<OpenUI>();
        if (openUI != null)
        {
            openUI.QuitUI(new PlayerInteractions());
        }
    }

    public void SelectionMultiple()
    {
        ListSelectedPhotos.Clear();
        selectionMultipleOn = !selectionMultipleOn;
        foreach (GameObject photo in sortAlbum.AlbumDisplayedPhotos)
        {
            GameObject frame = photo.transform.GetChild(1).gameObject;
            frame.GetComponent<PhotoFrame>().canBeSelected = selectionMultipleOn;
            frame.transform.GetChild(2).gameObject.SetActive(selectionMultipleOn);
            if(!selectionMultipleOn)
                frame.transform.GetChild(2).GetComponent<Toggle>().isOn = false;
        }
        SelectionMultipleOptions.SetActive(selectionMultipleOn);
    }
    
    public void QuitPhoto()
    {
        panelAlbum.SetActive(true);
        panelPhoto.SetActive(false);
    }
    
    public void ShowPhoto (Sprite photoSprite, PhotoInfos photoInfos)
    {
        // Change Panel
        panelPhoto.SetActive(true);
        // verifier si la photo est dans l'encyclop�die
        if (photoInfos.imageUsedInNotes)
        {
            // Ne pas activer le bouton supprimer
            PanelPhoto_ButtonDelete.SetActive(false);

            // Ne pas activer le bouton remplacer
            PanelPhoto_ButtonReplace.SetActive(false);
        }
        else
        {
            // Activer le bouton supprimer
            PanelPhoto_ButtonDelete.SetActive(true);

            // Activer le bouton remplacer
            PanelPhoto_ButtonReplace.SetActive(true);
        }

        PanelPhoto_Photo.GetComponent<Image>().sprite = photoSprite;
        // Turn off album Panel
        panelAlbum.SetActive(false);
    }


    public void DeletePhoto()
    {
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.DeletePicture(photo);
        }
        ListSelectedPhotos.Clear();
        sortAlbum.LoadUI();
        if(panelPhoto.activeSelf)
            QuitPhoto();
    }

    public void FavUnfavPhoto()
    {
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.ModifyPicture(photo, true);
        }
        ListSelectedPhotos.Clear();
        if(panelAlbum.activeSelf)
            sortAlbum.LoadUI();
    }
    
    public void ChangeUsedInNotes()
    {
        return;
        
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.ModifyPicture(photo, false, true);
        }
        ListSelectedPhotos.Clear();
        sortAlbum.LoadUI();
    }

    public void On_ChangeEncyclopediaPhotoClicked(GameObject photo)
    {
        /*
        foreach (GameObject picture in ListInEncyclopedia)
        {
            if (albumDictionary[picture].imageTag == albumDictionary[photo].imageTag)
            {
                ListInEncyclopedia.Remove(picture);
                // albumDictionary[picture].imageUsedInEncyclopedia = false;
                break;
            }
        }
        ListInEncyclopedia.Add(photo);
        ChangeEntryPhoto.AddPhotoToEntries(albumDictionary[photo]);
        // albumDictionary[photo].imageUsedInEncyclopedia = true;
        */
    }
}
