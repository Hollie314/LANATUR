using System;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Write Text Mission", story: "Write [Text] Mission on [UI]", category: "Action", id: "2110445049185d4c58b296b3fb38b73b")]
public partial class WriteTextMissionAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Text;
    [SerializeReference] public BlackboardVariable<TextMeshProUGUI> UI;

    protected override Status OnStart()
    {
        UI.Value.text = Text.Value;
        return Status.Running;
    }
}

