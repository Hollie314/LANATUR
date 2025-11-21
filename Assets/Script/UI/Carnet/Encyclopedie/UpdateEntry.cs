using UnityEngine;
using System.Collections.Generic;

public class UpdateEntry : MonoBehaviour
{
    [SerializeField] public List<EncyclopedieEntry> EncyclopedieEntry;

    public EncyclopedieEntry.UpdateTypes UpdateType;

    public List<string> NoteDeRen;
    public List<string> Caracteristique;
    public List<string> Anecdote;

    public List<Sprite> Photo;
    public List<Sprite> Dessin;
    public List<Sprite> DessinMignon;

    Game_Manager Game_Manager;

    private void Awake()
    {
        Game_Manager = FindFirstObjectByType<Game_Manager>();
    }

    public void UpdateEntry_Func()
    {
        for(int i = 0; i < EncyclopedieEntry.Count; i++)
        {
            SingleUpdate(i);
        }
    }

    private void SingleUpdate(int i)
    {
        if (EncyclopedieEntry[i] == null) { return; }
        if (EncyclopedieEntry[i].UpdatesDone.Contains(UpdateType)) { return; }
        else
        {
            EncyclopedieEntry[i].UpdatesDone.Add(UpdateType);
            if (!(NoteDeRen[i] == "")) { EncyclopedieEntry[i].NoteDeRen = NoteDeRen[i]; }
            if (!(Caracteristique[i] == "")) { EncyclopedieEntry[i].Caracteristique = Caracteristique[i]; }
            if (!(Anecdote[i] == "")) { EncyclopedieEntry[i].Anecdote = Anecdote[i]; }
            if (!(Photo[i] == null)) { EncyclopedieEntry[i].Photo = Photo[i]; }
            if (!(Dessin[i] == null)) { EncyclopedieEntry[i].Dessin = Dessin[i]; }
            if (!(DessinMignon[i] == null)) { EncyclopedieEntry[i].DessinMignon = DessinMignon[i]; }

            if (!Game_Manager.EncyclopedieEntries.Contains(EncyclopedieEntry[i]))
            {
                Game_Manager.EncyclopedieEntries.Add(EncyclopedieEntry[i]);
            }
        }
    }
}
