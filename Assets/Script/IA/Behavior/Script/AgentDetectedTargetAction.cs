using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
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
        Debug.Log("Detection status : running");
        Debug.Log(RangeDetector.Value.GameObjectsDetected.Count);
        List<GameObject> detectedToRemove = new List<GameObject>();
        foreach(GameObject detected in RangeDetector.Value.GameObjectsDetected)
        {
            Debug.Log($"Detected name : {detected.name} Detected tag : {detected.tag}");
            Debug.Log($"Target name : {Target.Value.name} Detected tag : {Target.Value.tag}");
            if (!detected.gameObject.activeSelf)
            {
                detectedToRemove.Add(detected);
            }
            else if(detected.tag == Target.Value.tag)
            {
                return Status.Success;
            }
        }
        foreach(GameObject detected in detectedToRemove)
        {
            RangeDetector.Value.GameObjectsDetected.Remove(detected);
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

