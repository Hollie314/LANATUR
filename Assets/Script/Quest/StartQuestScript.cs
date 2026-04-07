using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace Quests
{
    public class StartQuestScript : MonoBehaviour
    {
        [SerializeField] private List<QuestScriptable> listQuests = new List<QuestScriptable>();
        private static bool questAlreadyGiven = false;

        public void StartQuest()
        {
            if (questAlreadyGiven)
                return;
            if (listQuests.IsNullOrEmpty())
                return;
            foreach (QuestScriptable quest in listQuests)
            {
                GiveQuest(quest);
            }
        }
        
        [Button("tg")]
        public void StartGivingQuest(QuestScriptable quest)
        {
            GiveQuest(quest);
        }

        public static void GiveQuest(QuestScriptable _quest)
        {
            UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
            _updateQuestUI.StartQuestUI(_quest, 0, false);
            QuestManager _questManager = FindFirstObjectByType<QuestManager>();
            _questManager.activeQuests.Add(_quest);
            Debug.Log("Give Quest 1");

            if (_quest.questDialogue != null)
            {
                Debug.Log("conv start with quest");
                DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
                dialogueManager.StartConv(_quest.questDialogue);
            }
            
            switch (_quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle:
                    _quest.currentProgressionStr = _quest.GoToPoints[0];
                    break;
                case QuestScriptable.QuestType.GoToPoint:
                    _quest.currentProgressionStr = _quest.GoToPoints[0];
                    break;
                case QuestScriptable.QuestType.PhotographSpecie:
                    _quest.currentProgressionGO = _quest.SpeciesToPhotograph[0];
                    break;
            }
            
            Debug.Log("Give Quest 2");
            
            if (_quest.validateIfAlreadyCompleted)
            {
                Debug.Log("Give Quest 3");
                if(_quest.completeInOrder)
                    _questManager.CheckCompletionInOrder(_quest);
                else
                    _questManager.CheckCompletionAllOrder(_quest);
            }
            Debug.Log("Give Quest 4");
            
            questAlreadyGiven = true;
        }
    }   
}
