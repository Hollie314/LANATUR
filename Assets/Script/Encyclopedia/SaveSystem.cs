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
    }

    public static void DeletePicture()
    {
        // TO DO
    }

    public static void SavePosition()
    {
        // To Do
    }
}
