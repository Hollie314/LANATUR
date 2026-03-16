using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "detection", story: "Check for closest target in layer", category: "Action", id: "d024bc68f12566b4b483a6e2b2a03bc3")]
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