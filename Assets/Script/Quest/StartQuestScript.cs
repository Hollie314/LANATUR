using UnityEngine;

namespace Quests
{
    public class StartQuestScript : MonoBehaviour
    {
        [SerializeField] private QuestScriptable _quest;

        public void StartQuest()
        {
            UpdateQuestUI.StartQuestUI();
            QuestManager _questManager = FindFirstObjectByType<QuestManager>();
            _questManager.activeQuests.Add(_quest);
            
            if (_quest.validateIfAlreadyCompleted)
            {
                
            }
        }
    }   
}
