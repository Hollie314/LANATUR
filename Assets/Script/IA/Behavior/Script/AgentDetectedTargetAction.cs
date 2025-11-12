using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Agent detected Target", story: "[Agent] detected [target]", category: "Action", id: "e72ebd5be267d6aadbae88ae687fdaa4")]
public partial class AgentDetectedTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<RangeDetector> RangeDetector;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("it's running");
        if(RangeDetector.Value.GameObjectsDetected.Contains(Target))
            return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

