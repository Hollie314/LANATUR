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
    
    public List<QuestScriptable> QuestToGiveNext = new List<QuestScriptable>();
    public QuestType questType;
    
    public bool validateIfAlreadyCompleted;
    public bool completeInOrder;
    
    [SerializeField] private string questName;
    [SerializeField] private string questDesciption;
        
    [ShowIf("questType", QuestType.GoToPoint)] [ShowIf("questType", QuestType.CompletePuzzle)] 
    public List<string> GoToPoints;
    [ShowIf("questType", QuestType.PhotographSpecie)] public List<GameObject> SpeciesToPhotograph;
    [HideInInspector] public GameObject currentProgressionGO {get; set;}

    [HideInInspector] public string currentProgressionStr {get; set;}
    
    public List<GameObject> CompletedObjectivesGO = new List<GameObject>();  //Hide in inspector
    public List<string> CompletedObjectivesStr = new List<string>();  //Hide in inspector
}