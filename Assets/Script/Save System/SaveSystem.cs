using Sirenix.Utilities;
using System.IO;
using Unity.AppUI.Core;
using UnityEngine;

public static class SaveSystem
{
    private static string picturesFolder = Path.Combine(Application.persistentDataPath, "SavePicture");
    public static Album album;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]

    private static void Initialize()
    {
        if(!Directory.Exists(picturesFolder))
        {
            Directory.CreateDirectory(picturesFolder);
        }
        album = Album.Load();
        DeleteAlbum(album);
    }

    public static PhotoInfos SavePicture(Texture2D image, string tag, bool isInEncyclopedia, EncyclopedieEntry encyclopedieEntry, PhotoInfos.ImageTypes type)
    {
        album = Album.Load();
        byte[] png = image.EncodeToPNG();
        string now = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        string filePath = Path.Combine(picturesFolder, $"{now}.png");
        File.WriteAllBytes(filePath, png);

        PhotoInfos photoInfos = new PhotoInfos()
        {
            imagePath = filePath,
            imageTag = tag,
            imageUsedInNotes = isInEncyclopedia,
            entry = encyclopedieEntry,
            imageType = type,
            imageDate = System.DateTime.Now.ToBinary(),
            imageFav = false
        };

        album.photoInfos.Add(photoInfos);

        Album.Save(album);
        return photoInfos;
    }

    public static void DeletePicture(PhotoInfos infos)
    {
        album = Album.Load();
        album.photoInfos.Remove(infos);
        File.Delete(infos.imagePath);
        Album.Save(album);
    }

    public static void ModifyPicture(PhotoInfos infos, bool isFav = false, bool usedInNotes  = false )
    {
        album = Album.Load();
        album.photoInfos.Remove(infos);
        if(isFav)
            infos.imageFav = !infos.imageFav;
        if(usedInNotes)
            infos.imageUsedInNotes = !infos.imageUsedInNotes;
        album.photoInfos.Add(infos);
        Album.Save(album);
    }

    public static void DeleteAlbum(Album album)
    {
        album = Album.Load();
        foreach (PhotoInfos info in album.photoInfos)
        {
            DeletePicture(info);
        }
        album.photoInfos.Clear();
        Album.Save(album);
        File.Delete($"{Application.persistentDataPath}/album.json");
    }

    public static void SavePosition()
    {
        // To Do
    }
}
