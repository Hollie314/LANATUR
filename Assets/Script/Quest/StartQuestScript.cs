using UnityEngine;

namespace Quests
{
    public class StartQuestScript : MonoBehaviour
    {
        [SerializeField] private QuestScriptable _quest;

        public void StartQuest()
        {
            UpdateQuestUI _updateQuestUI = FindFirstObjectByType<UpdateQuestUI>();
            _updateQuestUI.StartQuestUI(_quest);
            QuestManager _questManager = FindFirstObjectByType<QuestManager>();
            _questManager.activeQuests.Add(_quest);
            
            if (_quest.validateIfAlreadyCompleted)
            {
                _questManager.CheckCompletion(_quest);
            }

            switch (_quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle:
                    _quest.currentProgressionGO = _quest.PuzzlesToComplete[0];
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
