using UnityEngine;
using DialogueEditor;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public NPCConversation conversation;
    public PhotoInfos conditions;
    public List<QuestScriptable> quests = new List<QuestScriptable>();
}
