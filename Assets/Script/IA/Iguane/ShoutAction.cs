using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shout", story: "[Self] [shout] [Time] second", category: "Action", id: "fe31607e47e2cc7c91baf98418e29793")]
public partial class ShoutAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<ShoutAtTarget> Shout;
    [SerializeReference] public BlackboardVariable<int> Time;
    protected override Status OnStart()
    {
        if (!Shout.Value.isShouting)
        {
            Shout.Value.TryShout(Time.Value, Self.Value);
            return Status.Success;
        }
        return Status.Failure;
    }

    
}

