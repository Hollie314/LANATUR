using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Sirenix.Utilities;

public class EncyclopedieUI : MonoBehaviour
{
    public Game_Manager Game_Manager;
    public int currentEntry;

    public Text NoteDeRen;
    public Text Caracteristique;
    public Text Anecdote;

    public Image Photo;
    public Image Dessin;
    public Image DessinMignon;

    public static Album album = new Album();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Game_Manager = FindFirstObjectByType<Game_Manager>();
        Debug.Log($"Manager name : {Game_Manager.name} - Encyclopedie Entries : {Game_Manager.EncyclopedieEntries} - Count : {Game_Manager.EncyclopedieEntries.Count}");
        currentEntry = 1;

        if (!Game_Manager.EncyclopedieEntries.IsNullOrEmpty())
        {
            UpdateUI(Game_Manager.EncyclopedieEntries[0]);
            ShowEncyclopedia();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToNext()
    {
        if(Game_Manager.EncyclopedieEntries.Count <= 1) { return; }

        if(Game_Manager.EncyclopedieEntries.Count == currentEntry + 1) 
        {
            currentEntry = 0;
        }
        else { currentEntry ++; }
        UpdateUI(Game_Manager.EncyclopedieEntries[currentEntry]);
        return;


    }

    public void GoToPrevious()
    {
        if (Game_Manager.EncyclopedieEntries.Count <= 1) { return; }

        if (currentEntry == 0)
        {
            currentEntry = Game_Manager.EncyclopedieEntries.Count - 1;
        }
        else { currentEntry --; }
        UpdateUI(Game_Manager.EncyclopedieEntries[currentEntry]);
        return;
    }

    private void UpdateUI(EncyclopedieEntry entry)
    {
        NoteDeRen.text = entry.NoteDeRen;
        Caracteristique.text = entry.Caracteristique;
        Anecdote.text = entry.Anecdote;

        Photo.sprite = entry.Photo;
        Dessin.sprite = entry.Dessin;
        DessinMignon.sprite = entry.DessinMignon;
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
                    foreach (EncyclopedieEntry entry in Game_Manager.EncyclopedieEntries)
                    {
                        Debug.Log("searching images");
                        Debug.Log(infos.imageTag);
                        Debug.Log(entry.specieTag);
                        if (entry.specieTag == infos.imageTag)
                        {
                            Debug.Log("adding photo in encyclopedia");
                            AddPhotoToEncyclopedia(infos, entry);
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
