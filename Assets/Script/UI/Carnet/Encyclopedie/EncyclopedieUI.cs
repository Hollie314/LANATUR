using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Sirenix.Utilities;

public class EncyclopedieUI : MonoBehaviour
{
    [SerializeField] private UI_Notifications _uiNotifications;
    
    public Game_Manager Game_Manager;
    public int currentEntry;

    public Text NoteDeRen;
    public Text Caracteristique;
    public Text Anecdote;

    public Image Photo;
    public Image Dessin;
    public Image DessinMignon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Game_Manager = FindFirstObjectByType<Game_Manager>();
        //Debug.Log($"Manager name : {Game_Manager.name} - Encyclopedie Entries : {Game_Manager.EncyclopedieEntries} - Count : {Game_Manager.EncyclopedieEntries.Count}");
        currentEntry = 1;

        if (!Game_Manager.EncyclopedieEntries.IsNullOrEmpty())
        {
            UpdateUI(Game_Manager.EncyclopedieEntries[0]);
        }
        
        _uiNotifications.ChangeCarnet(false);
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
}
