using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeDetectorForNavigation", story: "Update Range [Detector] assign [Target] check [Layer]", category: "Action", id: "ba2858077eb39c797fed346bfe4ba771")]
public partial class RangeDetectorForNavigationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<int> Layer;
    [SerializeReference] public BlackboardVariable<RangeDetection> Detector;
    protected override Status OnUpdate()
    {
        Target.Value = Detector.Value.UpdateDetector(Layer.Value);
        return Detector.Value.UpdateDetector(Layer.Value) == null ? Status.Failure : Status.Success;
    }
}

