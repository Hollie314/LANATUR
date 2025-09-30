using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]

public struct Album
{
    [SerializeField] public List<PhotoInfos> photoInfos;

    public static Album Load()
    {
        string filePath = $"{Application.persistentDataPath}/album.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<Album>(json);
        }
        return new Album() { photoInfos = new List<PhotoInfos>()};
    }

    public static void Save(Album album) 
    {
        string filePath = $"{Application.persistentDataPath}/album.json";
        string json = JsonUtility.ToJson(album);
        File.WriteAllText(filePath, json);
    }
}
