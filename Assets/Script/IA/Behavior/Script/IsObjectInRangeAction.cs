using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Is Appat in Range", story: "The [Appat] is in [RangeDetector]", category: "Action/Find", id: "f5e61eebda746fa05db229a312e21d0a")]
public partial class IsAppatInRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Appat;
    [SerializeReference] public BlackboardVariable<RangeDetector> RangeDetector;

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

