using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetector", story: "Update Range [Detector] assign [Target] check [Layer]", category: "Action", id: "d024bc68f12566b4b483a6e2b2a03bc3")]
public partial class RangeDetectorNavigationAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetection> Detector;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> Layer;

    protected override Status OnUpdate()
    {
        Target.Value = Detector.Value.UpdateDetector(Layer.Value);
        return Detector.Value.UpdateDetector(Layer.Value) == null ? Status.Failure : Status.Success;
    }
}

