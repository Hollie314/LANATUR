using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class EncyclopedieUI : MonoBehaviour
{
    public List<EncyclopedieEntry> EncyclopedieEntries;
    public int currentEntry;

    public Text NoteDeRen;
    public Text Caracteristique;
    public Text Anecdote;

    public Image Photo;
    public Image Dessin;
    public Image DessinMignon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (EncyclopedieEntries == null) { EncyclopedieEntries = new List<EncyclopedieEntry>(); }
        currentEntry = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToNext()
    {
        if(EncyclopedieEntries.Count <= 1) { return; }

        if(EncyclopedieEntries.Count == currentEntry + 1) 
        {
            currentEntry = 0;
        }
        else { currentEntry ++; }
        UpdateUI(EncyclopedieEntries[currentEntry]);
        return;


    }

    public void GoToPrevious()
    {
        if (EncyclopedieEntries.Count <= 1) { return; }

        if (currentEntry == 0)
        {
            currentEntry = EncyclopedieEntries.Count - 1;
        }
        else { currentEntry --; }
        UpdateUI(EncyclopedieEntries[currentEntry]);
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

    public void LoadFirstEntry()
    {
        if(EncyclopedieEntries.Count != 0)
        {
            UpdateUI(EncyclopedieEntries[0]);
        }
    }
}
