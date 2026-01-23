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
        }
    }   
}
