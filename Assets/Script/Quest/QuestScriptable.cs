using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]

public class QuestScriptable : ScriptableObject
{
    public enum QuestType
    {
        GoToPoint,
        CompletePuzzle,
        PhotographSpecie
    }
        
    public QuestType questType  {get; private set;}
    [SerializeField] private string questName;
    [SerializeField] private string questDesciption;
        
    [ShowIf("questType", QuestType.GoToPoint)] public List<GameObject> goToPoints {get; private set;}
    [ShowIf("questType", QuestType.CompletePuzzle)] public List<GameObject> PuzzlesToComplete {get; private set;}
    [ShowIf("questType", QuestType.PhotographSpecie)] public List<GameObject> SpeciesToPhotograph {get; private set;}
    [HideInInspector] public GameObject currentProgression {get; set;}
    public bool validateIfAlreadyCompleted {get; private set;}
}