using UnityEngine;

[CreateAssetMenu(fileName = "EncyclopedieEntry", menuName = "Scriptable Objects/EncyclopedieEntry")]
public class EncyclopedieEntry : ScriptableObject
{
    public string NoteDeRen;
    public string Caracteristique;
    public string Anecdote;

    public Sprite Photo;
    public Sprite Dessin;
    public Sprite DessinMignon;
}
