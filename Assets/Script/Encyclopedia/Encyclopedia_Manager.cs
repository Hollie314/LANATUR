using Sirenix.OdinInspector;
using Sirenix.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia_Manager : SerializedMonoBehaviour
{
    public Dictionary<GameObject, string> EncylopediaDictionary;
    public static Album album = new Album();

    public void ShowEncyclopedia()
    {
        album = Album.Load();
        if (!album.photoInfos.IsNullOrEmpty())
        {
            foreach (PhotoInfos infos in album.photoInfos)
            {
                if (infos.imageUsedInEncyclopedia)
                {
                    foreach (GameObject encyclopediaPhoto in EncylopediaDictionary.Keys)
                    {
                        if(encyclopediaPhoto.tag == infos.imageTag)
                        {
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

    public void ShowEncyclopediaDescription()
    {

    }

    public void ChangePage(int pageIndex)
    {

    }


}
