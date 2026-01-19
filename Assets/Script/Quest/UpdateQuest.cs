using UnityEngine;

namespace Quests
{
    public class UpdateQuest : MonoBehaviour
    {
        public void CheckCompletion(QuestScriptable quest)
        {
            // Access somewhere where events are stocked
            // Check if completion was already done
            switch (quest.questType)
            {
                case QuestScriptable.QuestType.CompletePuzzle :
                    // vérifier si l'objet précis currentprogression a envoyé un event
                    break;
                case QuestScriptable.QuestType.GoToPoint :
                        
                    break;
                case QuestScriptable.QuestType.PhotographSpecie :
                        
                    break;
            }
        }
    }
}
