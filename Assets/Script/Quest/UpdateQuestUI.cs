using TMPro;
using UnityEngine;

namespace Quests
{
    public class UpdateQuestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI inGameQuestUI;
        [SerializeField] private TextMeshProUGUI CarnetQuestUI;
        [SerializeField] private AudioSource questAudioSource;
        [SerializeField] private AudioClip questStartedSFX;
        [SerializeField] private AudioClip questUpdatedSFX;
        [SerializeField] private AudioClip questEndedSFX;
        
        public void StartQuestUI(QuestScriptable _quest)
        {
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questStartedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest);
            UpdateCarnetUI(_quest);
        }

        public void EndQuestUI(QuestScriptable _quest)
        {
            Debug.Log("quest: ended");
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questEndedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest);
            UpdateCarnetUI(_quest);
        }

        public void UpdateUI(QuestScriptable _quest)
        {
            if (!questAudioSource.isPlaying)
            {
                questAudioSource.clip = questUpdatedSFX;
                questAudioSource.Play();
            }
            UpdateInGameUI(_quest);
            UpdateCarnetUI(_quest);
        }

        private void UpdateCarnetUI(QuestScriptable _quest)
        {
            
        }

        private void UpdateInGameUI(QuestScriptable _quest)
        {
            inGameQuestUI.text = _quest.name;
        }
    }
}
