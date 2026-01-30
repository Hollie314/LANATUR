using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Quests
{
    public class UpdateQuestUI : MonoBehaviour
    {
        [SerializeField] private GameObject QuestInGameUI;
        [SerializeField] private GameObject QuestUpdateMessagePrefab;
        private GameObject activeMessage;
        [SerializeField] private float timeShowingMessage;
        private float timeActive;
        private List<GameObject> NextMessages = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI CarnetQuestUI;
        [SerializeField] private AudioSource questAudioSource;
        [SerializeField] private AudioClip questStartedSFX;
        [SerializeField] private AudioClip questUpdatedSFX;
        [SerializeField] private AudioClip questEndedSFX;
        
        public void StartQuestUI(QuestScriptable _quest, int progression,  bool completed)
        {
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questStartedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest, progression, completed);
            UpdateCarnetUI(_quest);
        }

        public void EndQuestUI(QuestScriptable _quest, int progression,  bool completed)
        {
            Debug.Log("quest: ended");
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questEndedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest, progression, completed);
            UpdateCarnetUI(_quest);
        }

        public void UpdateUI(QuestScriptable _quest, int progression,  bool completed)
        {
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questUpdatedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest, progression, completed);
            UpdateCarnetUI(_quest);
        }

        private void UpdateCarnetUI(QuestScriptable _quest)
        {
            
        }

        private void UpdateInGameUI(QuestScriptable _quest, int progression, bool completed)
        {
            NextMessages.Add(Instantiate(QuestUpdateMessagePrefab, QuestInGameUI.transform));
            NextMessages.Last().SetActive(false);
            NextMessages.Last().transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _quest.name;

            if (_quest.questType == QuestScriptable.QuestType.PhotographSpecie)
            {
                NextMessages.Last().transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"{progression} / {_quest.SpeciesToPhotograph.Count}";
            }
            else
            {
                NextMessages.Last().transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = $"{progression} / {_quest.GoToPoints.Count}";
            }
            
            if (completed)
            {
                NextMessages.Last().transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "Completed";
            }
        }

        private void Update()
        {
            if (activeMessage != null)
            {
                timeActive += Time.deltaTime;
                if (timeActive > timeShowingMessage)
                {
                    Destroy(activeMessage);
                    timeActive = 0;
                }
            }
            else if (!NextMessages.IsNullOrEmpty())
            {
                activeMessage = NextMessages[0];
                activeMessage.SetActive(true);
                NextMessages.RemoveAt(0);
            }
        }
    }
}
