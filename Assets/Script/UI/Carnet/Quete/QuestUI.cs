using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Sirenix.Utilities;

namespace UI.Quest
{
    public class QuestUI : MonoBehaviour
    {
        private Game_Manager Game_Manager;
        [SerializeField] private int currentEntry;

        [SerializeField] private Text NoteDeRen;
        [SerializeField] private Text Caracteristique;
        [SerializeField] private Text Anecdote;

        [SerializeField] private Image Photo;
        [SerializeField] private Image Dessin;
        [SerializeField] private Image DessinMignon;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            Game_Manager = FindFirstObjectByType<Game_Manager>();
            Debug.Log($"Manager name : {Game_Manager.name} - Encyclopedie Entries : {Game_Manager.EncyclopedieEntries} - Count : {Game_Manager.EncyclopedieEntries.Count}");
            currentEntry = 1;

            if (!Game_Manager.EncyclopedieEntries.IsNullOrEmpty()) // Change
            {
                UpdateUI(Game_Manager.EncyclopedieEntries[0]);
            }
        }

        public void GoToNext()
        {
            if(Game_Manager.EncyclopedieEntries.Count <= 1) { return; } // Change

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
            if (Game_Manager.EncyclopedieEntries.Count <= 1) { return; } // Change

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
}
