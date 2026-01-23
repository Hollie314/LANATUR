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

    public QuestType questType;
    public bool completeInOrder;
    [SerializeField] private string questName;
    [SerializeField] private string questDesciption;
        
    [ShowIf("questType", QuestType.GoToPoint)] public List<string> GoToPoints;
    [ShowIf("questType", QuestType.CompletePuzzle)] public List<GameObject> PuzzlesToComplete;
    [ShowIf("questType", QuestType.PhotographSpecie)] public List<GameObject> SpeciesToPhotograph;
    [HideInInspector] public GameObject currentProgressionGO {get; set;}
    [HideInInspector] public string currentProgressionStr {get; set;}
    [HideInInspector] public List<GameObject> CompletedObjectivesGO {get; set;} = new List<GameObject>();
    [HideInInspector] public List<string> CompletedObjectivesStr {get; set;} = new List<string>();
    public bool validateIfAlreadyCompleted {get; private set;}
}