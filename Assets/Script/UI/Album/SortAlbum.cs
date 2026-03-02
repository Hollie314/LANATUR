using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
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
    
    private List<PhotoInfos> AlbumSortedType1;
    private List<PhotoInfos> AlbumSortedType2;
    private Dictionary<PhotoInfos, GameObject> albumDictionary = new Dictionary<PhotoInfos, GameObject>();
    
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
    public void SortAll()
    {
        sortType1 = SortType1.All;
        
    }
    
    public void SortFav()
    {
        sortType1 = SortType1.Fav;
    }

    public void SortSpecies()
    {
        sortType1 = SortType1.Species;
    }
    
    public void SortEncyclopedia()
    {
        sortType1 = SortType1.Encyclopedia;
    }
    
    public void SortRen()
    {
        sortType1 = SortType1.Ren;
    }
    
    public void SortCorpo()
    {
        sortType1 = SortType1.Corporation;
    }
    #endregion
    
    #region SortType2
    public void SortByDate_OldestFirst()
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

    public void SortByDate_NewestFirst()
    {
        sortType2 = SortType2.NewestFirst;
        foreach (PhotoInfos photoInfo in AlbumSortedType1)
        {
            
        }
        ShowPhotos();
    }
    #endregion

    #region ShowPhotos
    private void ShowPhotos()
    {
        switch (albumSize)
        {
            case AlbumSize.Small:
                ShowSmall();
                break;
            case AlbumSize.Medium:
                ShowMedium();
                break;
            case AlbumSize.Large:
                ShowLarge();
                break;
        }
    }

    private GameObject CreatePhoto(PhotoInfos photoInfo)
    {
        GameObject photo = PhotoPrefab;
        if(photoInfo.imageUsedInEncyclopedia){}
        return photo;
    }

    private void ShowSmall()
    {
        //Utiliser un seul scroll
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
            
            Instantiate(photo);
        }
    }

    private void ShowMedium()
    {
        //Utiliser 2 scrolls
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
            
            if (index % 2 == 0)
            {
                
            }
            else{}
            index++;
        }
    }

    private void ShowLarge()
    {
        //Utiliser 3 scrolls
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
            
            if (index % 3 == 0)
            {
                
            }
            else if (index % 2 == 0)
            {
                
            }
            else{}
            index++;
        }
    }
    #endregion
}
