using Sirenix.OdinInspector;
using Sirenix.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEntryPhoto : SerializedMonoBehaviour
{
    public Dictionary<GameObject, string> EncylopediaDictionary;
    public static Album album = new Album();

    public void OnEnable()
    {
        ShowEncyclopedia();
    }

    public void ShowEncyclopedia()
    {
        Debug.Log("showing Encylopedia");
        album = Album.Load();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            Debug.Log("album not empty");
            foreach (PhotoInfos infos in album.photoInfos)
            {
                Debug.Log("searching photoinfos");
                if (infos.imageUsedInEncyclopedia)
                {
                    Debug.Log("info in encyclopedia");
                    foreach (GameObject encyclopediaPhoto in EncylopediaDictionary.Keys)
                    {
                        Debug.Log("searching images");
                        Debug.Log(infos.imageTag);
                        Debug.Log(encyclopediaPhoto.tag);
                        if (encyclopediaPhoto.tag == infos.imageTag)
                        {
                            Debug.Log("adding photo in encyclopedia");
                            AddPhotoToEncyclopedia(infos, encyclopediaPhoto);
                        }
                    }
                }
            }
        }
    }

    public void AddPhotoToEncyclopedia(PhotoInfos infos, GameObject photo)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(infos.imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        Sprite photoSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        photo.GetComponent<Image>().sprite = photoSprite;
        return;
    }
}
