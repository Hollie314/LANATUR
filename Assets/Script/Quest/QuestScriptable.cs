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
    
    public Dialogue questDialogue;
    
    public List<QuestScriptable> QuestToGiveNext = new List<QuestScriptable>();
    public QuestType questType;
    
    public bool validateIfAlreadyCompleted;
    public bool completeInOrder;
    
    public string questName;
    public string questDesciption;
    
    public List<string> GoToPoints;
    [ShowIf("questType", QuestType.PhotographSpecie)] public List<GameObject> SpeciesToPhotograph;

    [HideInInspector] public GameObject currentProgressionGO {get; set;}
    [HideInInspector] public string currentProgressionStr {get; set;}
    
    public List<GameObject> CompletedObjectivesGO = new List<GameObject>();  //Hide in inspector
    public List<string> CompletedObjectivesStr = new List<string>();  //Hide in inspector

    [HideInInspector] public int progression;
    [HideInInspector] public int MaxProgress = 1;
}