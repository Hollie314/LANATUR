using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EncyclopedieEntry", menuName = "Scriptable Objects/EncyclopedieEntry")]
public class EncyclopedieEntry : ScriptableObject
{
    public enum UpdateTypes { RenPage, SpecieScanned, Anecdote}

    public List<UpdateTypes> UpdatesDone = new List<UpdateTypes>();
    public string NoteDeRen;
    public string Caracteristique;
    public string Anecdote;

    public Sprite Photo;
    public Sprite Dessin;
    public Sprite DessinMignon;
}
