using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace Quests
{
    public class StartQuestScript : MonoBehaviour
    {
        [SerializeField] private List<QuestScriptable> listQuests = new List<QuestScriptable>();
        private bool questAlreadyGiven = false;

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

        public static void GiveQuest(QuestScriptable _quest)
        {
            UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
            _updateQuestUI.StartQuestUI(_quest);
            QuestManager _questManager = FindFirstObjectByType<QuestManager>();
            _questManager.activeQuests.Add(_quest);
            
            if (_quest.validateIfAlreadyCompleted)
            {
                if(_quest.completeInOrder)
                    _questManager.CheckCompletionInOrder(_quest);
                else
                    _questManager.CheckCompletionAllOrder(_quest);
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
        }
    }   
}
