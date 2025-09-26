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
            Debug.Log("picturesFolder created");
        }
        album = Album.Load();
    }

    public static void SavePicture(Texture2D image, string tag)
    {
        byte[] png = image.EncodeToPNG();
        string now = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        string filePath = Path.Combine(picturesFolder, $"{now}.png");
        File.WriteAllBytes(filePath, png);

        PhotoInfos photoInfos = new PhotoInfos()
        {
            imagePath = filePath,
            imageTag = tag
        };

        album.photoInfos.Add(photoInfos);

        Album.Save(album);
        Debug.Log("Picture saved");
        Debug.Log("PhotoInfos saved " + photoInfos.imagePath + " " + photoInfos.imageTag);
        Debug.Log("Album PhotoInfos in SaveSystem is null" + album.photoInfos.IsNullOrEmpty());
    }

    public static void DeletePicture(PhotoInfos infos)
    {
        File.Delete(infos.imagePath);
        album.photoInfos.Remove(infos);
    }

    public static void SavePosition()
    {
        // To Do
    }
}
