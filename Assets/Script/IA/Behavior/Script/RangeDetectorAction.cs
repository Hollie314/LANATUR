using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetector", story: "Check in Range [Detector] the closest Target in [Layer]", category: "Action", id: "1e1c56034c4e8f962af9266e24ecd759")]
public partial class RangeDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetection> Detector;
    [SerializeReference] public BlackboardVariable<int> Layer;

    protected override Status OnUpdate()
    {
        // Just check if there is any object in the layer
        GameObject detected = Detector.Value.UpdateDetector(Layer.Value);
        return detected == null ? Status.Failure : Status.Success;
    }
}

