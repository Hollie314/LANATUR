using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "run while bool is value", story: "run while [bool] is [value]", category: "Action", id: "117263100b9e08422ff3d8b5a7329065")]
public partial class WhileBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Bool;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Bool == null)
        {
            return Status.Failure;
        }
        else if (Bool != Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

