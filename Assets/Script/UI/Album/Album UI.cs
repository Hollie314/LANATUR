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
    [SerializeField] private ChangeEntryPhoto ChangeEntryPhoto;
    [SerializeField] private SortAlbum _sortAlbum;

    [Header("Canvas / Panels")]
    [SerializeField] private GameObject canvasCamera;
    [SerializeField] private GameObject panelAlbum, panelPhoto;
    
    [Header("Buttons")]
    [SerializeField] private GameObject PanelPhoto_Photo;
    [SerializeField] private GameObject PanelPhoto_ButtonDelete;
    [SerializeField] private GameObject PanelPhoto_ButtonReplace;
    [SerializeField] private GameObject SelectionMultipleOptions;

    private GameObject PhotoZoomedOn;
    private List<GameObject> ListSelectedPhotos = new List<GameObject>();
    private bool selectionMultipleOn;

    public void On_BackToCamera()
    {
        canvasCamera.SetActive(true);
        DestroyImages();
        this.gameObject.SetActive(false);
    }

    public void On_PhotoClicked(GameObject photoClicked)
    {
        return;
        if (selectionMultipleOn)
        {
            if ((ListSelectedPhotos.IsNullOrEmpty() || !ListSelectedPhotos.Contains(photoClicked)) && !ListInEncyclopedia.Contains(photoClicked))
            {
                ListSelectedPhotos.Add(photoClicked);
                photoClicked.gameObject.transform.GetChild(2).gameObject.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                ListSelectedPhotos.Remove(photoClicked);
                photoClicked.gameObject.transform.GetChild(2).gameObject.GetComponent<Toggle>().isOn = false;
            }
        }
        else
        {
            ListSelectedPhotos.Clear();
            ListSelectedPhotos.Add(photoClicked);
            // Change Panel
            panelPhoto.SetActive(true);
            // v�rifier si la photo est dans l'encyclop�die
            if (ListInEncyclopedia != null)
            {
                if (ListInEncyclopedia.Contains(photoClicked))
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
            }

            PanelPhoto_Photo.GetComponent<Image>().sprite = photoClicked.GetComponent<Image>().sprite;

            // Turn off album Panel
            panelAlbum.SetActive(false);
        }
    }

    public void On_BackToAlbumClicked()
    {
        panelAlbum.SetActive(true);

        panelPhoto.SetActive(false);
    }

    public void On_DeletePhotoClicked()
    {
        foreach (var photo in ListSelectedPhotos)
        {
            SaveSystem.DeletePicture(albumDictionary[photo]);
        }

        ListSelectedPhotos.Clear ();

        DestroyImages();

        SortByType();

        On_BackToAlbumClicked();
    }

    public void On_ChangeEncyclopediaPhotoClicked(GameObject photo)
    {
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
    }

    public void On_SelectionMultipleClicked()
    {
        ListSelectedPhotos.Clear();
        selectionMultipleOn = !selectionMultipleOn;
        SelectionMultipleOptions.SetActive(selectionMultipleOn);
    }
}
