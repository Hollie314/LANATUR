using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumUI : MonoBehaviour
{
    public GameObject panelAlbum, panelPhoto;
    public TextMeshProUGUI picturesNumber;

    public enum SortMethodes {Encyclopedia, Species, Date}
    public SortMethodes SortMethodeUsed;

    public static Album album = new Album();
    public GameObject AlbumLayout;
    public GameObject PicturePrefab;

    private Dictionary<GameObject, PhotoInfos> albumDictionary = new Dictionary<GameObject, PhotoInfos>();

    private static void Initialize()
    {

    }

    public void OnEnable()
    {
        SortByType();
    }

    #region changeSortMethodes
    public void ChangeSortMethodeToEncyclopedia()
    {
        SortMethodeUsed = SortMethodes.Encyclopedia;
        SortByType();
    }

    public void ChangeSortMethodeToSpecies()
    {
        SortMethodeUsed = SortMethodes.Species;
        SortByType();
    }

    public void ChangeSortMethodeToDate()
    {
        SortMethodeUsed = SortMethodes.Date;
        SortByType();
    }
    #endregion

    #region Sort
    public void SortByType()
    {
        album = Album.Load();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            switch (SortMethodeUsed)
            {
                case SortMethodes.Encyclopedia:
                    SortByEncyclopedia();
                    break;

                case SortMethodes.Species:
                    SortBySpecies();
                    break;

                case SortMethodes.Date:
                    SortByDate();
                    break;
            }
        }
    }

    public void SortByEncyclopedia()
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

    public void SortBySpecies()
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
            byte[] bytes = System.IO.File.ReadAllBytes(infos.imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            Sprite photoSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            newPicture.GetComponent<Image>().sprite = photoSprite;

            // Update Photo UI
            UpdatePhotoUI(newPicture);

            // Update Dictionary
            albumDictionary.Add(newPicture, infos);
        }
        // Update picturesNumber
        picturesNumber.text = $"{index}/150"; // change later with {maxPictures}
    }

    public void UpdatePhotoUI(GameObject photo)
    {
        // photo index
        // photo on encyclopedia
        // photo species
    }
    #endregion

    public void OnPhotoClicked()
    {
        // Change Panel
        panelPhoto.SetActive(true);

        // Turn off album Panel
        panelAlbum.SetActive(false);
    }

    public void DeletePhoto(GameObject photo)
    {
        SaveSystem.DeletePicture(albumDictionary[photo]);
        Destroy(photo);
    }
}
