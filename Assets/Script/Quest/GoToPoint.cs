using System;
using Quests;
using UnityEngine;

public class GoToPoint : MonoBehaviour
{
    [SerializeField] private bool startsQuest;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (startsQuest)
            {
                this.gameObject.GetComponent<StartQuestScript>().StartQuest();
            }
            // Update
            this.gameObject.GetComponent<UpdateQuest>().UpdateQuestProgress();
        }
    }
}
