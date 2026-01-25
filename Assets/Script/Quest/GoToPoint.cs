using System;
using Quests;
using UnityEngine;

public class GoToPoint : MonoBehaviour
{
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (startsQuest)
                this.gameObject.GetComponent<StartQuestScript>().StartQuest();
            
            if (updateQuest)
                this.gameObject.GetComponent<UpdateQuest>().UpdateQuestProgress();
        }
    }
}
