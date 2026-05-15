using System;
using Quests;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("quete trigger entré");
        if (other.tag == "Player")
        {
            Debug.Log("quete trigger entré tag player détecté");
            if (startsQuest){
                Debug.Log("quete trigger va start quest");
                this.gameObject.GetComponent<StartQuestScript>().StartQuest();
                Debug.Log("quete trigger a start quest");
            }
            if (updateQuest){
                this.gameObject.GetComponent<UpdateQuest>().UpdateQuestProgress();
            }
        }
    }
}
