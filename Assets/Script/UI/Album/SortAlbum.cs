using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

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
    
    private List<PhotoInfos> AlbumSortedType1;
    private List<PhotoInfos> AlbumSortedType2;
    private Dictionary<PhotoInfos, GameObject> albumDictionary = new Dictionary<PhotoInfos, GameObject>();

    private GameObject scroll1 = new GameObject();
    private GameObject scroll2 = new GameObject();
    private GameObject scroll3 = new GameObject();
    
    // When Album is Enabled, reset it in case new photos appeared
    void OnEnable()
    {
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
                SortEncyclopedia();
                break;
            case SortType1.Ren:
                SortRen();
                break;
            case SortType1.Corporation:
                SortCorpo();
                break;
        }
    }

    #region SortType1
    public void Sort1()
    {
        switch (DropdownSortType1.value)
        {
            case 0:
                SortAll();
                break;
            case 1:
                SortFav();
                break;
            case 2:
                SortSpecies();
                break;
            case 3:
                SortEncyclopedia();
                break;
            case 4:
                SortRen();
                break;
            case 5:
                SortCorpo();
                break;
        }
    }
    
    private void SortAll()
    {
        sortType1 = SortType1.All;
        Sort2(false);
    }
    
    private void SortFav()
    {
        sortType1 = SortType1.Fav;
        Sort2(false);
    }

    private void SortSpecies()
    {
        sortType1 = SortType1.Species;
        Sort2(false);
    }
    
    private void SortEncyclopedia()
    {
        sortType1 = SortType1.Encyclopedia;
        Sort2(false);
    }
    
    private void SortRen()
    {
        sortType1 = SortType1.Ren;
        Sort2(false);
    }
    
    private void SortCorpo()
    {
        sortType1 = SortType1.Corporation;
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
                    SortByDate_OldestFirst();
                    break;
                case 1:
                    SortByDate_NewestFirst();
                    break;
            }
        }
        else
        {
            switch (sortType2)
            {
                case SortType2.OldestFirst:
                    SortByDate_OldestFirst();
                    break;
                case SortType2.NewestFirst:
                    SortByDate_NewestFirst();
                    break;
            }
        }
    }
    
    private void SortByDate_OldestFirst()
    {
        sortType2 = SortType2.OldestFirst;
        
        int lastphotoDate = 0;
        bool isSorted = false;
        int numberOfLoops = 0;
        
        while (!isSorted || numberOfLoops < 500)
        {
            List<PhotoInfos> sortingList = new List<PhotoInfos>();
            int index = 0;
            // Parcourir chaque photos à afficher
            foreach (PhotoInfos photoInfo in AlbumSortedType1)
            {
                isSorted = true;
                sortingList.Add(photoInfo);
                if (index > 1)
                {
                    // Si la photo précédente est plus récente:
                    if (photoInfo.imageDate > lastphotoDate)
                    {
                        // inverser l'ordre dans la liste
                        PhotoInfos previous = sortingList[index-1];
                        sortingList[index-1] = photoInfo;
                        sortingList[index] = previous;
                        isSorted = false;
                    }
                    lastphotoDate = sortingList[index].imageDate;
                }
                index++;
            }
            AlbumSortedType2 = sortingList;
            numberOfLoops++;
        }
        
        ShowPhotos();
    }

    private void SortByDate_NewestFirst()
    {
        sortType2 = SortType2.NewestFirst;
        foreach (PhotoInfos photoInfo in AlbumSortedType1)
        {
            
        }
        ShowPhotos();
    }
    #endregion

    #region AlbumSize
    public void SortAlbumSize()
    {
        switch (DropdownSortType2.value)
        {
            case 0:
                SizeSmall();
                break;
            case 1:
                SizeMedium();
                break;
            case 2:
                SizeLarge();
                break;
        }
    }
    
    private void SizeSmall()
    {
        albumSize = AlbumSize.Small;
        DestroyScrollview();
        
        //Utiliser un seul scroll
        scroll1 = ScrollVerticalLayout.transform.GetChild(0).gameObject;
        
        ShowPhotos();
    }

    private void SizeMedium()
    {
        albumSize = AlbumSize.Small;
        DestroyScrollview();
        
        //Utiliser 2 scrolls
        scroll1 = ScrollVerticalLayout.transform.GetChild(0).gameObject;
        scroll2 = Instantiate(new GameObject(), ScrollVerticalLayout.transform);
        
        ShowPhotos();
    }

    private void SizeLarge()
    {
        albumSize = AlbumSize.Small;
        DestroyScrollview();
        
        //Utiliser 3 scrolls
        scroll1 = ScrollVerticalLayout.transform.GetChild(0).gameObject;
        scroll2 = Instantiate(new GameObject(), ScrollVerticalLayout.transform);
        scroll3 = Instantiate(new GameObject(), ScrollVerticalLayout.transform);
        
        ShowPhotos();
    }
    
    private void DestroyScrollview()
    {
        for (int i = 1; i < ScrollVerticalLayout.transform.childCount; i++)
        {
            Destroy(ScrollVerticalLayout.transform.GetChild(i).gameObject);
        }
    }
    #endregion

    #region ShowPhotos
    private void ShowPhotos()
    {
        int index = 0;
        foreach (PhotoInfos photoInfo in AlbumSortedType2)
        {
            // Vérifier si la photo existe déjà en mémoire
            GameObject photo = new GameObject();
            if (albumDictionary.ContainsKey(photoInfo)) { photo = albumDictionary[photoInfo]; }
            else
            {
                photo = CreatePhoto(photoInfo);
                albumDictionary.Add(photoInfo, photo);
            }
            
            if (index % 3 == 0 && (albumSize == AlbumSize.Large))
                Instantiate(photo, scroll3.transform);
            else if (index % 2 == 0 && (albumSize == AlbumSize.Large || albumSize == AlbumSize.Medium))
                Instantiate(photo, scroll2.transform);
            else
                Instantiate(photo, scroll1.transform);
            index++;
        }
    }

    private GameObject CreatePhoto(PhotoInfos photoInfo)
    {
        GameObject photo = PhotoPrefab;
        if(photoInfo.imageUsedInEncyclopedia){}
        return photo;
    }
    #endregion
}