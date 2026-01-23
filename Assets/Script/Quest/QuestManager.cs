using UnityEngine;
using System.Collections.Generic;
using Quests;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private UpdateQuest _updateQuest;
    public List<GameObject> gameObjectsThatSentEvents = new List<GameObject>();
    public List<string> eventsStringReceived = new List<string>();
    public List<string> speciesInPhoto = new List<string>();
    public List<string> eventsReceived = new List<string>();
    public List<QuestScriptable> activeQuests = new List<QuestScriptable>();

    private void OnEnable()
    {
        // += events
        Camera_Shot.SpecieTakenInPhoto += SpeciesEvent;
        UpdateQuest.objectiveUpdated += GoToEvent;
    }

    private void OnDisable()
    {
        // -= events
        Camera_Shot.SpecieTakenInPhoto -= SpeciesEvent;
        UpdateQuest.objectiveUpdated -= GoToEvent;
    }
    

    #region events

    private void SpeciesEvent(string specie)
    {
        speciesInPhoto.Add(specie);
        Debug.Log("quest: recieved specie: " + specie);
        UpdateQuests();
    }
    
    private void GoToEvent(string objectiveUpdated)
    {
        eventsReceived.Add(objectiveUpdated);
        Debug.Log("quest: recieved specie: " + objectiveUpdated);
        UpdateQuests();
    }

    #endregion

    private void UpdateQuests()
    {
        foreach (QuestScriptable quest in activeQuests)
        {
            CheckCompletion(quest);
        }
    }
    
    public void CheckCompletion(QuestScriptable quest)
    {
        // Access somewhere where events are stocked
        // Check if completion was already done
        if (quest.completeInOrder)
        {
            // vérifier si l'objet précis currentprogression a envoyé un event
            if (gameObjectsThatSentEvents.Contains(quest.currentProgressionGO) || eventsStringReceived.Contains(quest.currentProgressionStr))
            {
                UpdateProgressionInOrder(quest);
            }
        }
        else
        {
            switch (quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle :
                    // vérifier si n'importe quel objectif a envoyé un event
                    foreach (GameObject objective in quest.PuzzlesToComplete)
                    {
                        if (gameObjectsThatSentEvents.Contains(objective) || eventsStringReceived.Contains(quest.currentProgressionStr))
                        {
                            UpdateProgressionPuzzle(quest, objective);
                        }
                    }
                    break;
            
                case QuestScriptable.QuestType.GoToPoint :
                    foreach (string objective in quest.GoToPoints)
                    {
                        if (eventsReceived.Contains(objective))
                        {
                            UpdateProgressionGoToPoint(quest, objective);
                        }
                    }
                    break;
            
                case QuestScriptable.QuestType.PhotographSpecie :
                    foreach (GameObject objective in quest.SpeciesToPhotograph)
                    {
                        if (speciesInPhoto.Contains(objective.tag))
                        {
                            UpdateProgressionPhotographSpecie(quest, objective);
                        }
                    }
                    break;
            }
        }
    }

    private void UpdateProgressionInOrder(QuestScriptable quest)
    {
        int index = 0;
        switch (quest.questType)
        {
            case QuestScriptable.QuestType.CompletePuzzle:
                index = quest.PuzzlesToComplete.IndexOf((quest.currentProgressionGO));
                if (index >= quest.PuzzlesToComplete.Count)
                {
                    // end quest
                    return;
                }

                quest.currentProgressionGO = quest.PuzzlesToComplete[index + 1];
                // Show progression
                return;

            case QuestScriptable.QuestType.GoToPoint:
                index = quest.GoToPoints.IndexOf((quest.currentProgressionStr));
                if (index >= quest.GoToPoints.Count)
                {
                    // end quest
                    return;
                }

                quest.currentProgressionStr = quest.GoToPoints[index + 1];
                // Show progression
                return;

            case QuestScriptable.QuestType.PhotographSpecie:
                index = quest.SpeciesToPhotograph.IndexOf((quest.currentProgressionGO));
                if (index >= quest.SpeciesToPhotograph.Count)
                {
                    // end quest
                    return;
                }

                quest.currentProgressionGO = quest.SpeciesToPhotograph[index + 1];
                // Show progression
                return;
        }
    }

    private void UpdateProgressionGoToPoint(QuestScriptable quest, string objectiveCompleted)
    {
        quest.CompletedObjectivesStr.Add(objectiveCompleted);
        bool questEnded = false;
        if (quest.CompletedObjectivesStr.Count == quest.GoToPoints.Count)
        {
            questEnded = true;
        }
    }
    
    private void UpdateProgressionPuzzle(QuestScriptable quest, GameObject objectiveCompleted)
    {
        
    }
    
    private void UpdateProgressionPhotographSpecie(QuestScriptable quest, GameObject objectiveCompleted)
    {
        quest.CompletedObjectivesGO.Add(objectiveCompleted);
        bool questEnded = false;
        switch (quest.questType)
        {
            case QuestScriptable.QuestType.CompletePuzzle:
                if (quest.CompletedObjectivesGO.Count == quest.PuzzlesToComplete.Count)
                {
                    questEnded = true;
                }
                break;

            case QuestScriptable.QuestType.GoToPoint:
                if (quest.CompletedObjectivesGO.Count == quest.GoToPoints.Count)
                {
                    questEnded = true;
                }
                break;

            case QuestScriptable.QuestType.PhotographSpecie:
                if (quest.CompletedObjectivesGO.Count == quest.SpeciesToPhotograph.Count)
                {
                    questEnded = true;
                }
                break;
        }

        UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
        if (questEnded)
        {
            _updateQuestUI.EndQuestUI(quest);
            return;
        }
        _updateQuestUI.UpdateUI(quest);
    }
}
