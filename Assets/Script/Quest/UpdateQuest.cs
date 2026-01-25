using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Quests
{
    public class UpdateQuest : MonoBehaviour
    {
        [SerializeField] private QuestScriptable _quest;
        [SerializeField] private string ObjectiveString;
        
        public static event Action<string> objectiveUpdated;

        public void UpdateQuestProgress()
        {
            objectiveUpdated?.Invoke(ObjectiveString);
        }
    }
}
