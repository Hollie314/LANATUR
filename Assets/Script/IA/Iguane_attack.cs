using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check PlayerDetection true ", story: "If [PlayerDetection] play [AnimalBehavior]", category: "Action", id: "078ad8790016fc8828196cf477c3a19a")]
public partial class Iguane_attack : Action
{
    [SerializeReference] public BlackboardVariable<PlayerDetection> PlayerDetection;
    [SerializeReference] public BlackboardVariable<AnimalBehavior> AnimalBehavior;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

