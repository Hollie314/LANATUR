using Sirenix.OdinInspector;
using Sirenix.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEntryPhoto : SerializedMonoBehaviour
{

    public void AddPhotoToEntries(PhotoInfos infos)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(infos.imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        Sprite photoSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        infos.entry.Photo = photoSprite;
        return;
    }
}
