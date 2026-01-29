using UnityEngine;
using System.Collections.Generic;
using Quests;
using Sirenix.Utilities;

public class QuestManager : MonoBehaviour
{
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
        if(!speciesInPhoto.Contains(specie))
            speciesInPhoto.Add(specie);
        Debug.Log("quest: recieved specie: " + specie);
        
        // Update uniquement cet event
        if(!activeQuests.IsNullOrEmpty())
            UpdateQuests(specie);
    }
    
    private void GoToEvent(string objectiveUpdated)
    {
        if(!eventsReceived.Contains(objectiveUpdated))
            eventsReceived.Add(objectiveUpdated);
        Debug.Log("quest: recieved event: " + objectiveUpdated);
        
        // Update uniquement cet event
        if(!activeQuests.IsNullOrEmpty())
            UpdateQuests(objectiveUpdated);
    }

    #endregion

    private void UpdateQuests(string objectiveUpdated = null)
    {
        foreach (QuestScriptable quest in activeQuests)
        {
            if (quest.completeInOrder)
                CheckCompletionInOrder(quest, objectiveUpdated);
            else
                CheckCompletionAllOrder(quest, objectiveUpdated);
            if (activeQuests.IsNullOrEmpty())
            {
                return;
            }
        }
    }

    public void CheckCompletionInOrder(QuestScriptable quest, string objectiveEventStr = null, GameObject objectiveEventGO = null)
    {
        // vérifier si l'objet précis currentprogression a envoyé un event
        if (quest.validateIfAlreadyCompleted)
        {
            if (speciesInPhoto.Contains(quest.currentProgressionGO.tag) || eventsReceived.Contains(quest.currentProgressionStr))
            {
                UpdateProgressionInOrder(quest);
                if(activeQuests.Contains(quest))
                    CheckCompletionInOrder(quest, objectiveEventStr, objectiveEventGO);
            }
        }
        else
        {
            Debug.Log("quest INORDER 1");
            if (objectiveEventStr == (quest.currentProgressionGO.tag))
            {
                Debug.Log("quest INORDER 2");
                UpdateProgressionInOrder(quest);
            }
        }
    }
    
    public void CheckCompletionAllOrder(QuestScriptable quest, string objectiveEventStr = null, GameObject objectiveEventGO = null)
    {
        // Access somewhere where events are stocked
        // Check if completion was already done
        if (quest.validateIfAlreadyCompleted)
        {
            switch (quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle :
                    foreach (string objective in quest.GoToPoints)
                    {
                        if (eventsReceived.Contains(objective) && !quest.CompletedObjectivesStr.Contains(objective))
                        {
                            UpdateProgression(quest, objective);
                        }
                    }
                    break;
            
                case QuestScriptable.QuestType.GoToPoint :
                    foreach (string objective in quest.GoToPoints)
                    {
                        if (eventsReceived.Contains(objective) && !quest.CompletedObjectivesStr.Contains(objective))
                        {
                            UpdateProgression(quest, objective);
                        }
                    }
                    break;
            
                case QuestScriptable.QuestType.PhotographSpecie :
                    foreach (GameObject objective in quest.SpeciesToPhotograph)
                    {
                        if (speciesInPhoto.Contains(objective.tag))
                        {
                            UpdateProgression(quest, objective.tag);
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle :
                    if (quest.GoToPoints.Contains(objectiveEventStr) && !quest.CompletedObjectivesStr.Contains(objectiveEventStr))
                    {
                        UpdateProgression(quest, objectiveEventStr);
                    }
                    break;
                
                case QuestScriptable.QuestType.GoToPoint :
                    if (quest.GoToPoints.Contains(objectiveEventStr) && !quest.CompletedObjectivesStr.Contains(objectiveEventStr))
                    {
                        UpdateProgression(quest, objectiveEventStr);
                    }
                    break;
                
                case QuestScriptable.QuestType.PhotographSpecie :
                    foreach (GameObject specie in quest.SpeciesToPhotograph)
                    {
                        if(specie.tag == objectiveEventStr && !quest.CompletedObjectivesStr.Contains(objectiveEventStr))
                        {
                            UpdateProgression(quest, objectiveEventStr);
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
                Debug.Log("quest order 3");
                index = quest.GoToPoints.IndexOf((quest.currentProgressionStr));
                Debug.Log($"quest index :{index}");
                if (index + 1 > quest.GoToPoints.Count - 1)
                {
                    Debug.Log("quest order 4");
                    // end quest
                    EndQuest(quest);
                    return;
                }

                quest.currentProgressionStr = quest.GoToPoints[index + 1];
                // Show progression
                /*
                UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
                _updateQuestUI.UpdateUI(quest);
                */
                return;

            case QuestScriptable.QuestType.GoToPoint:
                Debug.Log("quest order 3");
                index = quest.GoToPoints.IndexOf((quest.currentProgressionStr));
                Debug.Log($"quest index :{index}");
                if (index + 1 > quest.GoToPoints.Count - 1)
                {
                    Debug.Log("quest order 4");
                    // end quest
                    EndQuest(quest);
                    return;
                }

                quest.currentProgressionStr = quest.GoToPoints[index + 1];
                // Show progression
                /*
                UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
                _updateQuestUI.UpdateUI(quest);
                */
                return;

            case QuestScriptable.QuestType.PhotographSpecie:
                Debug.Log("quest INORDER 3");
                index = quest.SpeciesToPhotograph.IndexOf((quest.currentProgressionGO));
                if (index + 1 > quest.SpeciesToPhotograph.Count - 1)
                {
                    // end quest
                    EndQuest(quest);
                    return;
                }

                quest.currentProgressionGO = quest.SpeciesToPhotograph[index + 1];
                Debug.Log($"quest currentprogression: {quest.currentProgressionGO.tag}");
                // Show progression
                return;
        }
    }

    private void UpdateProgression(QuestScriptable quest, string objectiveCompleted)
    {
        UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
        
        switch (quest.questType)
        {
            case QuestScriptable.QuestType.CompletePuzzle:
                Debug.Log("l'update a eu lieu");
                quest.CompletedObjectivesStr.Add(objectiveCompleted);
                Debug.Log($"completed objectives: {quest.CompletedObjectivesStr.Count}");
                Debug.Log($"All objectives: {quest.GoToPoints.Count}");
                if (quest.CompletedObjectivesStr.Count >= quest.GoToPoints.Count)
                {
                    EndQuest(quest);
                    Debug.Log("quest: 1");
                    return;
                }

                _updateQuestUI.UpdateUI(quest);
                return;
            
            case QuestScriptable.QuestType.PhotographSpecie:
                Debug.Log("l'update a eu lieu");
                quest.CompletedObjectivesStr.Add(objectiveCompleted);
                Debug.Log($"quest completed objectives: {quest.CompletedObjectivesStr.Count}");
                Debug.Log($"quest All objectives: {quest.SpeciesToPhotograph.Count}");
                if (quest.CompletedObjectivesStr.Count >= quest.SpeciesToPhotograph.Count)
                {
                    EndQuest(quest);
                    Debug.Log("quest: 1");
                    return;
                }

                _updateQuestUI.UpdateUI(quest);
                return;
            
            case QuestScriptable.QuestType.GoToPoint:
                Debug.Log("l'update a eu lieu");
                quest.CompletedObjectivesStr.Add(objectiveCompleted);
                Debug.Log($"completed objectives: {quest.CompletedObjectivesStr.Count}");
                Debug.Log($"All objectives: {quest.GoToPoints.Count}");
                if (quest.CompletedObjectivesStr.Count >= quest.GoToPoints.Count)
                {
                    EndQuest(quest);
                    Debug.Log("quest: 1");
                    return;
                }

                _updateQuestUI.UpdateUI(quest);
                return;
        }
    }

    private void EndQuest(QuestScriptable quest)
    {
        Debug.Log("End quest");
        UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
        _updateQuestUI.EndQuestUI(quest);
        Debug.Log("Frr ???");
        
        quest.CompletedObjectivesStr.Clear();
        quest.CompletedObjectivesGO.Clear();
        activeQuests.Remove(quest);
        
        if (quest.QuestToGiveNext.IsNullOrEmpty())
            return;
        foreach (QuestScriptable nextQuest in quest.QuestToGiveNext)
        {
            StartQuestScript.GiveQuest(nextQuest);
        }
        
    }
}
