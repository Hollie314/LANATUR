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
        Debug.Log($"path: {picturesFolder}");
        if(!Directory.Exists(picturesFolder))
        {
            Directory.CreateDirectory(picturesFolder);
            Debug.Log("picturesFolder created");
        }
        album = Album.Load();
        DeleteAlbum(album);
    }

    public static PhotoInfos SavePicture(Texture2D image, string tag, bool isInEncyclopedia, EncyclopedieEntry encyclopedieEntry)
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
            imageUsedInEncyclopedia = isInEncyclopedia,
            entry = encyclopedieEntry
        };

        album.photoInfos.Add(photoInfos);

        Album.Save(album);
        Debug.Log("PhotoInfos saved " + photoInfos.imagePath + " " + photoInfos.imageTag);
        return photoInfos;
    }

    public static void DeletePicture(PhotoInfos infos)
    {
        album = Album.Load();
        Debug.Log("SaveSystem Delete Picture Started");
        album.photoInfos.Remove(infos);
        File.Delete(infos.imagePath);
        Album.Save(album);
        Debug.Log($"Photo infos still in album: { album.photoInfos.Contains(infos)}");
        Debug.Log("SaveSystem Delete Picture Ended");
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
        Debug.Log("Deleted album");
    }

    public static void SavePosition()
    {
        // To Do
    }
}
