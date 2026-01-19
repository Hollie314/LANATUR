using UnityEngine;
using System.Collections.Generic;
using Quests;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private UpdateQuest _updateQuest;
    public List<GameObject> gameObjectsThatSentEvents = new List<GameObject>();
    public List<QuestScriptable> activeQuests = new List<QuestScriptable>();

    private void OnEnable()
    {
        // += events
    }

    private void OnDisable()
    {
        // -= events
    }

    private void AddEvent()
    {
        
    }

    private void UpdateQuests()
    {
        foreach (QuestScriptable quest in activeQuests)
        {
            _updateQuest.CheckCompletion(quest);
        }
    }
    
    public void CheckCompletion(QuestScriptable quest)
    {
        // Access somewhere where events are stocked
        // Check if completion was already done
        switch (quest.questType)
        {
            case QuestScriptable.QuestType.CompletePuzzle :
                // vérifier si l'objet précis currentprogression a envoyé un event
                if (gameObjectsThatSentEvents.Contains(quest.currentProgression))
                {
                    UpdateProgression(quest);
                }
                break;
            
            case QuestScriptable.QuestType.GoToPoint :
                if (gameObjectsThatSentEvents.Contains(quest.currentProgression))
                {
                    UpdateProgression(quest);
                }
                break;
            
            case QuestScriptable.QuestType.PhotographSpecie :
                if (gameObjectsThatSentEvents.Contains(quest.currentProgression))
                {
                    UpdateProgression(quest);
                }
                break;
        }
    }

    private void UpdateProgression(QuestScriptable quest)
    {
        // get current progression index compared to the list
    }
}
