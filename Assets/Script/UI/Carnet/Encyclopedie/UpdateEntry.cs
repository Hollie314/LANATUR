using UnityEngine;

public class UpdateEntry : MonoBehaviour
{
    [SerializeField] EncyclopedieEntry EncyclopedieEntry;

    public string NoteDeRen;
    public string Caracteristique;
    public string Anecdote;

    public Sprite Photo;
    public Sprite Dessin;
    public Sprite DessinMignon;

    public void UpdateEntry_Func()
    {
        if(!(NoteDeRen == "")) { EncyclopedieEntry.NoteDeRen = NoteDeRen; }
        if(!(Caracteristique == "")) { EncyclopedieEntry.Caracteristique = Caracteristique; }
        if(!(Anecdote == "")) { EncyclopedieEntry.Anecdote = Anecdote; }
        if(!(Photo == null)) { EncyclopedieEntry.Photo = Photo; }
        if(!(Dessin == null)) { EncyclopedieEntry.Dessin = Dessin; }
        if(!(DessinMignon == null)) { EncyclopedieEntry.DessinMignon = DessinMignon; }
    }
}
