using Unity.AppUI.Core;
using UnityEngine;
[System.Serializable]
public struct PhotoInfos
{
    public enum ImageTypes
    {
        Ren,
        Corpo,
        Encyclopedia,
        None
    };
    
    [SerializeField] public string imagePath;
    public long imageDate;
    [SerializeField] public string imageTag;
    [SerializeField] public EncyclopedieEntry entry;
    [SerializeField] public bool imageUsedInNotes;
    [SerializeField] public bool imageFav;
    [SerializeField] public ImageTypes imageType;
}
