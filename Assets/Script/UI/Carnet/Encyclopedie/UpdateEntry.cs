using UnityEngine;

public class UpdateEntry : MonoBehaviour
{
    [SerializeField] EncyclopedieEntry EncyclopedieEntry;

    public EncyclopedieEntry.UpdateTypes UpdateType;

    public string NoteDeRen;
    public string Caracteristique;
    public string Anecdote;

    public Sprite Photo;
    public Sprite Dessin;
    public Sprite DessinMignon;

    public void UpdateEntry_Func()
    {
        if (EncyclopedieEntry == null) { return; }
        if (EncyclopedieEntry.UpdatesDone.Contains(UpdateType)) { return; }
        else
        {
            EncyclopedieEntry.UpdatesDone.Add(UpdateType);
            if (!(NoteDeRen == "")) { EncyclopedieEntry.NoteDeRen = NoteDeRen; }
            if (!(Caracteristique == "")) { EncyclopedieEntry.Caracteristique = Caracteristique; }
            if (!(Anecdote == "")) { EncyclopedieEntry.Anecdote = Anecdote; }
            if (!(Photo == null)) { EncyclopedieEntry.Photo = Photo; }
            if (!(Dessin == null)) { EncyclopedieEntry.Dessin = Dessin; }
            if (!(DessinMignon == null)) { EncyclopedieEntry.DessinMignon = DessinMignon; }
        }
    }
}
