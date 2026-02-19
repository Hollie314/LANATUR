using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Increment", story: "Add [Number] to [Integer]", category: "Action", id: "34b03bbc43eaa57370cf44b4e3741f5b")]
public partial class IncrementAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Number;
    [SerializeReference] public BlackboardVariable<int> Integer;

    protected override Status OnStart()
    {
        Integer.Value += Number.Value;
        return Status.Success;
    }
}

