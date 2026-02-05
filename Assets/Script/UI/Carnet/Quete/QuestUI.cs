using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Sirenix.Utilities;

namespace UI.Quest
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private GameObject QuestPrefab;
        [SerializeField] private GameObject QuestLayoutLeft, QuestLayoutRight;
        
        private QuestManager Quest_Manager;
        [SerializeField] private int currentPage;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            Quest_Manager = FindFirstObjectByType<QuestManager>();
            Debug.Log($"Manager name : {Quest_Manager.name} - Quests : {Quest_Manager.activeQuests} - Count : {Quest_Manager.activeQuests.Count}");
            currentPage = 1;

            if (!Quest_Manager.activeQuests.IsNullOrEmpty()) // Change
            {
                UpdateUI(currentPage);
            }
        }

        public void GoToNext()
        {
            if (Quest_Manager.activeQuests.Count < currentPage * 6 + 1)
            {
                currentPage = 1;
                UpdateUI(currentPage);
                return;
            }
            currentPage++;
            return;
        }

        public void GoToPrevious() // Bug
        {
            if (currentPage <= 1)
            {
                currentPage = Mathf.CeilToInt(Quest_Manager.activeQuests.Count / 6) +1;
                Debug.Log($"Current page : {currentPage}");
                UpdateUI(currentPage);
                return;
            }
            currentPage--;
            return;
        }

        private void DestroyPage()
        {
            for (int i = 0; i < QuestLayoutLeft.transform.childCount; i++)
            {
                Destroy(QuestLayoutLeft.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < QuestLayoutRight.transform.childCount; i++)
            {
                Destroy(QuestLayoutRight.transform.GetChild(i).gameObject);
            }
        }

        private void UpdateUI(int page)
        {
            DestroyPage();
            int index = (page -1)*6 +1;
            
            Debug.Log("index: " + index);
            for (int i = 0; i < 6; i++)
            {
                if(Quest_Manager.activeQuests.Count < index) {return;}

                if (i < 3)
                {
                    GameObject quest = Instantiate(QuestPrefab, QuestLayoutLeft.transform);
                    quest.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Quest_Manager.activeQuests[index -1].questName;
                    quest.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Quest_Manager.activeQuests[index -1].questDesciption;
                    quest.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{Quest_Manager.activeQuests[index -1].progression} / {Quest_Manager.activeQuests[index -1].MaxProgress}";
                }
                else
                {
                    GameObject quest = Instantiate(QuestPrefab, QuestLayoutRight.transform);
                    quest.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Quest_Manager.activeQuests[index -1].questName;
                    quest.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Quest_Manager.activeQuests[index -1].questDesciption;
                    quest.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{Quest_Manager.activeQuests[index -1].progression} / {Quest_Manager.activeQuests[index -1].MaxProgress}";
                }

                index++;
            }
        }
    }
}
