using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumUI : SerializedMonoBehaviour
{
    [SerializeField] ChangeEntryPhoto ChangeEntryPhoto;

    public GameObject canvasCamera;
    public GameObject panelAlbum, panelPhoto;
    public GameObject albumLayoutUp, albumLayoutDown;
    public TextMeshProUGUI picturesNumber;
    public Dictionary<string, Sprite> tagVignettes;
    public Sprite EncyclopediaVignette;
    public GameObject PanelPhoto_Photo;
    public GameObject PanelPhoto_ButtonDelete;
    public GameObject PanelPhoto_ButtonReplace;

    private GameObject PhotoZoomedOn;

    public enum SortMethodes {Encyclopedia, Species, Date}
    public SortMethodes SortMethodeUsed;

    public static Album album = new Album();
    public GameObject AlbumLayout;
    public GameObject PicturePrefab;

    private Dictionary<GameObject, PhotoInfos> albumDictionary = new Dictionary<GameObject, PhotoInfos>();
    private List<GameObject> ListInEncyclopedia = new List<GameObject>();
    private List<GameObject> ListSelectedPhotos = new List<GameObject>();
    private bool selectionMultipleOn;
    public GameObject SelectionMultipleOptions;

    private static void Initialize()
    {

    }

    public void OnEnable()
    {
        SortByType();
        ChangeEntryPhoto = FindFirstObjectByType<ChangeEntryPhoto>();
    }

    #region changeSortMethodes
    public void ChangeSortMethodeToEncyclopedia()
    {
        SortMethodeUsed = SortMethodes.Encyclopedia;
        DestroyImages();
        SortByType();
    }

    public void ChangeSortMethodeToSpecies()
    {
        SortMethodeUsed = SortMethodes.Species;
        DestroyImages();
        SortByType();
    }

    public void ChangeSortMethodeToDate()
    {
        SortMethodeUsed = SortMethodes.Date;
        DestroyImages();
        SortByType();
    }
    #endregion

    #region Sort
    public void SortByType()
    {
        Debug.Log("sorting Album started");
        album = Album.Load();
        Debug.Log($"Album count = {album.photoInfos.Count}");
        if (!album.photoInfos.IsNullOrEmpty())
        {
            switch (SortMethodeUsed)
            {
                case SortMethodes.Encyclopedia:
                    ShowAlbumPhotos(SortByEncyclopedia());
                    break;

                case SortMethodes.Species:
                    ShowAlbumPhotos(SortBySpecies());
                    break;

                case SortMethodes.Date:
                    SortByDate();
                    break;
            }
        }
        Debug.Log("sorting Album ended");
    }

    public List<PhotoInfos> SortByEncyclopedia()
    {
        // Sort in a dictionary
        List<PhotoInfos> sortedSpecies = SortBySpecies();
        Dictionary<string, List<PhotoInfos>> EncyclopediaDictionary = new Dictionary<string, List<PhotoInfos>>();
        EncyclopediaDictionary.Add("encyclopedia", new List<PhotoInfos>());
        EncyclopediaDictionary.Add("not encyclopedia", new List<PhotoInfos>());
        foreach (PhotoInfos infos in sortedSpecies)
        {
            if(infos.imageUsedInEncyclopedia == true)
            {
                EncyclopediaDictionary["encyclopedia"].Add(infos);
            }
            else
            {
                EncyclopediaDictionary["not encyclopedia"].Add(infos);
            }
        }

        // Sort the dictionary into a list
        List<PhotoInfos> SortedPhotoInfos = new List<PhotoInfos>();
        foreach (List<PhotoInfos> infosList in EncyclopediaDictionary.Values)
        {
            foreach (PhotoInfos infos in infosList)
            {
                SortedPhotoInfos.Add(infos);
            }
        }

        // Show
        return SortedPhotoInfos;
    }

    public List<PhotoInfos> SortBySpecies()
    {
        // Sort in a dictionary
        Dictionary<string, List<PhotoInfos>> SpeciesDictionary = new Dictionary<string, List<PhotoInfos>>();
        foreach (PhotoInfos infos in album.photoInfos)
        {
            if (!SpeciesDictionary.ContainsKey(infos.imageTag))
            {
                List<PhotoInfos> photoInfosList = new List<PhotoInfos>();
                photoInfosList.Add(infos);
                SpeciesDictionary.Add(infos.imageTag, photoInfosList);
            }
            else
            {
                SpeciesDictionary[infos.imageTag].Add(infos);
            }
        }

        // Sort the dictionary into a list
        List<PhotoInfos> SortedPhotoInfos = new List<PhotoInfos>();
        foreach (List<PhotoInfos> infosList in SpeciesDictionary.Values)
        {
            foreach (PhotoInfos infos in infosList)
            {
                SortedPhotoInfos.Add(infos);
            }
        }

        // Show
        return SortedPhotoInfos;
    }

    public void SortByDate()
    {
        // Sort in a dictionary
        Dictionary<string, List<PhotoInfos>> SpeciesDictionary = new Dictionary<string, List<PhotoInfos>>();
        foreach (PhotoInfos infos in album.photoInfos)
        {
            if (!SpeciesDictionary.ContainsKey(infos.imageTag))
            {
                List<PhotoInfos> photoInfosList = new List<PhotoInfos>();
                photoInfosList.Add(infos);
                SpeciesDictionary.Add(infos.imageTag, photoInfosList);
            }
            else
            {
                SpeciesDictionary[infos.imageTag].Add(infos);
            }
        }

        // Sort the dictionary into a list
        List<PhotoInfos> SortedPhotoInfos = new List<PhotoInfos>();
        foreach (List<PhotoInfos> infosList in SpeciesDictionary.Values)
        {
            foreach (PhotoInfos infos in infosList)
            {
                SortedPhotoInfos.Add(infos);
            }
        }

        // Show
        ShowAlbumPhotos(SortedPhotoInfos);
    }

    public void ShowAlbumPhotos(List<PhotoInfos> SortedPhotoInfos)
    {
        albumDictionary.Clear();
        int index = 0;
        GameObject newPicture;

        foreach (PhotoInfos infos in SortedPhotoInfos)
        {
            Debug.Log("Updating Album");

            // Create picture
            if (index % 2 == 0)
            {
                if (index == 0)
                {
                    newPicture = PicturePrefab;
                }
                else
                {
                    newPicture = Instantiate(PicturePrefab, AlbumLayout.transform.GetChild(0));
                }
            }
            else
            {
                newPicture = Instantiate(PicturePrefab, AlbumLayout.transform.GetChild(1));
            }
            index++;

            // Load image
            Debug.Log("about to load bytes");
            byte[] bytes = System.IO.File.ReadAllBytes(infos.imagePath);
            Debug.Log("Loaded bytes");
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            Sprite photoSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            newPicture.GetComponent<Image>().sprite = photoSprite;

            // Update Photo UI
            UpdatePhotoUI(newPicture, infos.imageTag, infos);

            // Update Dictionary
            albumDictionary.Add(newPicture, infos);

            if (infos.imageUsedInEncyclopedia)
                ListInEncyclopedia.Add(newPicture);
        }
        // Update picturesNumber
        picturesNumber.text = $"{index}/150"; // change later with {maxPictures}
    }

    public void UpdatePhotoUI(GameObject photo, string tag, PhotoInfos infos)
    {
        // photo index
        // photo on encyclopedia
        if (infos.imageUsedInEncyclopedia)
        {
            GameObject vignette = photo.transform.GetChild(0).gameObject;
            vignette.SetActive(true);
            vignette.GetComponent<Image>().sprite = EncyclopediaVignette;
        }
        else
        {
            GameObject vignette = photo.transform.GetChild(0).gameObject;
            vignette.SetActive(false);
        }

        // photo species
        Debug.Log(tag);
        if(tagVignettes.ContainsKey(tag))
        {
            GameObject vignette = photo.transform.GetChild(1).gameObject;
            vignette.SetActive(true);
            vignette.GetComponent<Image>().sprite = tagVignettes[tag];
        }
        else
        {
            GameObject vignette = photo.transform.GetChild(1).gameObject;
            vignette.SetActive(false);
        }
    }
    #endregion

    public void On_BackToCamera()
    {
        canvasCamera.SetActive(true);
        DestroyImages();
        this.gameObject.SetActive(false);
    }

    public void On_PhotoClicked(GameObject photoClicked)
    {
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
            // vérifier si la photo est dans l'encyclopédie
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

    public void DestroyImages()
    {
        for (int i = 1; i < albumLayoutUp.transform.childCount; i++)
        {
            Destroy(albumLayoutUp.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < albumLayoutDown.transform.childCount; i++)
        {
            Destroy(albumLayoutDown.transform.GetChild(i).gameObject);
        }
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
