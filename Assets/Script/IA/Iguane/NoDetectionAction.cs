using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NoDetection", story: "Perception [Detection] is false", category: "Action", id: "eec5f57b6cc4b76f9b97bcce8187a0d8")]
public partial class NoDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<PerceptionRange> Detection;

    protected override Status OnStart()
    {
        Detection.Value.StopDetection();
        return Status.Success;
    }
    
}

