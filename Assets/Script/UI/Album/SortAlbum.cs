using System;
using System.Collections.Generic;
using RTGStandard;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.UI;

public class SortAlbum : MonoBehaviour
{
    private enum AlbumSize
    {
        Small,
        Medium,
        Large,
    };
    private enum SortType1
    {
        All,
        Fav,
        Species,
        Encyclopedia,
        Ren,
        Corporation,
    };
    private enum SortType2
    {
        OldestFirst,
        NewestFirst,
    };
    
    [Header("Sorting infos")]
    [SerializeField] [ReadOnly] private SortType1 sortType1 = SortType1.All;
    [SerializeField] [ReadOnly] private SortType2 sortType2 = SortType2.NewestFirst;
    [SerializeField] [ReadOnly] private AlbumSize albumSize = AlbumSize.Medium;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject ScrollVerticalLayout;
    [SerializeField] private GameObject PhotoPrefab;
    [SerializeField] private TMP_Dropdown DropdownSize;
    [SerializeField] private TMP_Dropdown DropdownSortType1;
    [SerializeField] private TMP_Dropdown DropdownSortType2;
    [SerializeField] private TextMeshProUGUI photoNumber;
    
    [Header("Photo scales")]
    [SerializeField] private Vector2 smallPhotoScale;
    [SerializeField] private Vector2 mediumPhotoScale;
    [SerializeField] private Vector2 largePhotoScale;
    
    private List<PhotoInfos> AlbumSortedType1;
    private List<PhotoInfos> AlbumSortedType2;
    private Dictionary<PhotoInfos, GameObject> albumDictionary = new Dictionary<PhotoInfos, GameObject>();
    [HideInInspector] public List<GameObject> AlbumDisplayedPhotos = new List<GameObject>();

    private GameObject scroll1;
    private GameObject scroll2;
    private GameObject scroll3;

    private Vector2 currentPhotoScale;
    
    private Album album = new Album();
    
    private void Awake()
    {
        album = Album.Load();
        scroll1 = ScrollVerticalLayout.transform.GetChild(0).gameObject;
        scroll2 = ScrollVerticalLayout.transform.GetChild(1).gameObject;
        scroll3 = ScrollVerticalLayout.transform.GetChild(2).gameObject;
    }
    
    // When Album is Enabled, reset it in case new photos appeared
    private void OnEnable()
    {
        LoadUI();
    }

    public void LoadUI()
    {
        album = Album.Load();
        
        UpdateNumber();
        
        foreach (PhotoInfos photoInfos in album.photoInfos)
        {
            Debug.Log($"photo date: {photoInfos.imageDate}");
        }
        
        switch (albumSize)
        {
            case AlbumSize.Large:
                SizeLarge();
                break;
            case AlbumSize.Medium:
                SizeMedium();
                break;
            case AlbumSize.Small:
                SizeSmall();
                break;
        }
        
        switch (sortType1)
        {
            case SortType1.All:
                SortAll();
                break;
            case SortType1.Fav:
                SortFav();
                break;
            case SortType1.Species:
                SortSpecies();
                break;
            case SortType1.Encyclopedia:
                SortNotes();
                break;
            case SortType1.Ren:
                SortRen();
                break;
            case SortType1.Corporation:
                SortCorpo();
                break;
        }
    }

    public void UpdateNumber()
    {
        photoNumber.text = $"{album.photoInfos.Count.ToString()} / 150";
    }

    #region SortType1
    public void Sort1()
    {
        switch (DropdownSortType1.value)
        {
            case 0:
                Debug.Log("Sort All");
                SortAll();
                break;
            case 1:
                Debug.Log("Sort Fav");
                SortFav();
                break;
            case 2:
                Debug.Log("Sort Species");
                SortSpecies();
                break;
            case 3:
                Debug.Log("Sort Notes");
                SortNotes();
                break;
            case 4:
                Debug.Log("Sort Ren");
                SortRen();
                break;
            case 5:
                Debug.Log("Sort Corpo");
                SortCorpo();
                break;
        }
    }
    
    private void SortAll()
    {
        sortType1 = SortType1.All;
        AlbumSortedType1 = album.photoInfos;
        Sort2(false);
    }
    
    private void SortFav()
    {
        sortType1 = SortType1.Fav;
        AlbumSortedType1 = new List<PhotoInfos>();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in album.photoInfos)
            {
                if(photoInfo.imageFav)
                    AlbumSortedType1.Add(photoInfo);
            }
        }
        Sort2(false);
    }

    private void SortSpecies()
    {
        sortType1 = SortType1.Species;
        AlbumSortedType1 = new List<PhotoInfos>();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in album.photoInfos)
            {
                if(photoInfo.imageType == PhotoInfos.ImageTypes.Encyclopedia)
                    AlbumSortedType1.Add(photoInfo);
            }
        }
        Sort2(false);
    }
    
    private void SortNotes()
    {
        sortType1 = SortType1.Encyclopedia;
        AlbumSortedType1 = new List<PhotoInfos>();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in album.photoInfos)
            {
                if(photoInfo.imageUsedInNotes)
                    AlbumSortedType1.Add(photoInfo);
            }
        }
        Sort2(false);
    }
    
    private void SortRen()
    {
        sortType1 = SortType1.Ren;
        AlbumSortedType1 = new List<PhotoInfos>();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in album.photoInfos)
            {
                if(photoInfo.imageType == PhotoInfos.ImageTypes.Ren)
                    AlbumSortedType1.Add(photoInfo);
            }
        }
        Sort2(false);
    }
    
    private void SortCorpo()
    {
        sortType1 = SortType1.Corporation;
        AlbumSortedType1 = new List<PhotoInfos>();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in album.photoInfos)
            {
                if(photoInfo.imageType == PhotoInfos.ImageTypes.Corpo)
                    AlbumSortedType1.Add(photoInfo);
            }
        }
        Sort2(false);
    }
    #endregion
    
    #region SortType2
    public void Sort2(bool calledInUI)
    {
        if (calledInUI)
        {
            switch (DropdownSortType2.value)
            {
                case 0:
                    Debug.Log("Sort Newest First");
                    SortByDate_NewestFirst();
                    break;
                case 1:
                    Debug.Log("Sort Oldest First");
                    SortByDate_OldestFirst();
                    break;
            }
        }
        else
        {
            switch (sortType2)
            {
                case SortType2.NewestFirst:
                    SortByDate_NewestFirst();
                    break;
                case SortType2.OldestFirst:
                    SortByDate_OldestFirst();
                    break;
            }
        }
    }
    
    private void SortByDate_OldestFirst()
    {
        sortType2 = SortType2.OldestFirst;
        
        Debug.Log(DateTime.Now);
        long lastphotoDate = new DateTime(2025, 01, 01, 01, 01, 01, 01).ToBinary();
        bool isSorted = false;
        int numberOfLoops = 0;
        AlbumSortedType2 = AlbumSortedType1;
        
        while (!isSorted || numberOfLoops < 40)
        {
            List<PhotoInfos> sortingList = new List<PhotoInfos>();
            int index = 0;
            Debug.Log($"photo date old1st: index[{numberOfLoops}]");
            isSorted = true;
            // Parcourir chaque photos à afficher
            if (!AlbumSortedType1.IsNullOrEmpty())
            {
                foreach (PhotoInfos photoInfo in AlbumSortedType2)
                {
                    sortingList.Add(photoInfo);
                    if (index > 0)
                    {
                        // Si la photo précédente est plus récente:
                        if (DateTime.FromBinary(photoInfo.imageDate) > DateTime.FromBinary(lastphotoDate))
                        {
                            Debug.Log($"photo date old1st: Inversed indexes{lastphotoDate}");
                            // inverser l'ordre dans la liste
                            PhotoInfos previous = sortingList[index-1];
                            sortingList[index-1] = photoInfo;
                            sortingList[index] = previous;
                            isSorted = false;
                        }
                    }
                    lastphotoDate = sortingList[index].imageDate;
                    index++;
                }
            }
            AlbumSortedType2 = sortingList;
            numberOfLoops++;
            Debug.Log(numberOfLoops);
            if (numberOfLoops >= 60){break;}
            if(isSorted){break;}
        }
        Debug.Log("photo date old1st: finished");
        foreach (PhotoInfos photoInfos in AlbumSortedType2)
        {
            Debug.Log($"photo date old1st: {photoInfos.imageDate}");
        }
        DestroyScrollview();
        ShowPhotos();
    }

    private void SortByDate_NewestFirst()
    {
        sortType2 = SortType2.NewestFirst;
        
        Debug.Log(DateTime.Now);
        long lastphotoDate = new DateTime(2025, 01, 01, 01, 01, 01, 01).ToBinary();
        Debug.Log($"photo date last new1st: {lastphotoDate}");
        bool isSorted = false;
        int numberOfLoops = 0;
        AlbumSortedType2 = AlbumSortedType1;
        
        while (!isSorted || numberOfLoops < 40)
        {
            List<PhotoInfos> sortingList = new List<PhotoInfos>();
            int index = 0;
            isSorted = true;
            // Parcourir chaque photos à afficher
            if (!AlbumSortedType1.IsNullOrEmpty())
            {
                foreach (PhotoInfos photoInfo in AlbumSortedType2)
                {
                    sortingList.Add(photoInfo);
                    if (index > 0)
                    {
                        // Si la photo précédente est plus récente:
                        if (DateTime.FromBinary(photoInfo.imageDate) < DateTime.FromBinary(lastphotoDate))
                        {
                            Debug.Log("Inversed indexes");
                            // inverser l'ordre dans la liste
                            PhotoInfos previous = sortingList[index-1];
                            sortingList[index-1] = photoInfo;
                            sortingList[index] = previous;
                            isSorted = false;
                        }
                    }
                    lastphotoDate = sortingList[index].imageDate;
                    index++;
                }
            }
            AlbumSortedType2 = sortingList;
            numberOfLoops++;
            Debug.Log(numberOfLoops);
            if (numberOfLoops >= 60){return;}
            if(isSorted){break;}
        }
        foreach (PhotoInfos photoInfos in AlbumSortedType2)
        {
            Debug.Log($"photo date new1st: {photoInfos.imageDate}");
        }
        DestroyScrollview();
        ShowPhotos();
    }
    #endregion

    #region AlbumSize
    public void SortAlbumSize()
    {
        switch (DropdownSize.value)
        {
            case 0:
                Debug.Log("Sort Album Small");
                SizeSmall();
                break;
            case 1:
                Debug.Log("Sort Album Medium");
                SizeMedium();
                break;
            case 2:
                Debug.Log("Sort Album Large");
                SizeLarge();
                break;
        }
    }
    
    private void SizeSmall()
    {
        albumSize = AlbumSize.Small;
        currentPhotoScale = smallPhotoScale;
        DestroyScrollview();
        
        scroll2.SetActive(false);
        scroll3.SetActive(false);
        
        ShowPhotos();
    }

    private void SizeMedium()
    {
        albumSize = AlbumSize.Medium;
        currentPhotoScale = mediumPhotoScale;
        DestroyScrollview();
        
        //Utiliser 2 scrolls
        scroll2.SetActive(true);
        scroll3.SetActive(false);
        
        ShowPhotos();
    }

    private void SizeLarge()
    {
        albumSize = AlbumSize.Large;
        currentPhotoScale = largePhotoScale;
        DestroyScrollview();
        
        //Utiliser 3 scrolls
        scroll2.SetActive(true);
        scroll3.SetActive(true);
        
        ShowPhotos();
    }
    
    private void DestroyScrollview()
    {
        for (int i = 0; i < scroll1.transform.childCount; i++)
        {
            Destroy(scroll1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < scroll2.transform.childCount; i++)
        {
            Destroy(scroll2.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < scroll3.transform.childCount; i++)
        {
            Destroy(scroll3.transform.GetChild(i).gameObject);
        }
    }
    #endregion

    #region ShowPhotos
    private void ShowPhotos()
    {
        AlbumDisplayedPhotos.Clear();
        int index = 1;
        if (!AlbumSortedType2.IsNullOrEmpty())
        {
            foreach (PhotoInfos photoInfo in AlbumSortedType2)
            {
                // Vérifier si la photo existe déjà en mémoire
                GameObject photo = new GameObject();
                // if (albumDictionary.ContainsKey(photoInfo)) { photo = albumDictionary[photoInfo]; }
                photo = CreatePhoto(photoInfo);
                photo.GetComponent<RectTransform>().sizeDelta = currentPhotoScale;

                if (index % 3 == 0 && (albumSize == AlbumSize.Large))
                {
                    AlbumDisplayedPhotos.Add(Instantiate(photo, scroll3.transform));
                    Debug.Log($"scroll 3");
                }
                else if (index % 2 == 0 && (albumSize == AlbumSize.Large || albumSize == AlbumSize.Medium))
                {
                    AlbumDisplayedPhotos.Add(Instantiate(photo, scroll2.transform));
                    Debug.Log($"scroll 2");
                }
                else
                {
                    AlbumDisplayedPhotos.Add(Instantiate(photo, scroll1.transform));
                    Debug.Log($"scroll 1");
                }
                index++;
                Debug.Log($"index show photo: {index}");
            }
        }
        Debug.Log($"album null: {album.photoInfos.IsNullOrEmpty()}, sortType1: {AlbumSortedType1.IsNullOrEmpty()}, sortType2: {AlbumSortedType2.IsNullOrEmpty()}");
        if(!album.photoInfos.IsNullOrEmpty() && !AlbumSortedType1.IsNullOrEmpty() && !AlbumSortedType2.IsNullOrEmpty())
            Debug.Log($"album count: {album.photoInfos.Count}, sortType1: {AlbumSortedType1.Count}, sortType2: {AlbumSortedType2.Count}");
    }

    private GameObject CreatePhoto(PhotoInfos photoInfo)
    {
        GameObject photo = PhotoPrefab;
        // Load image
        byte[] bytes = System.IO.File.ReadAllBytes(photoInfo.imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        Sprite photoSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photo.transform.GetChild(1).GetComponent<Image>().sprite = photoSprite;
        photo.transform.GetChild(1).GetComponent<PhotoFrame>().photoInfos = photoInfo;
        
        if (photoInfo.imageUsedInNotes)
        {
            photo.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
        }
        
        if (!(photoInfo.imageType == PhotoInfos.ImageTypes.None))
        {
            photo.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
        }

        if (photoInfo.imageFav)
        {
            photo.transform.GetChild(2).gameObject.SetActive(true);
        }
        
        return photo;
    }
    #endregion
}