using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SwitchLayerAction", story: "[Self] switch to layer [Layer]", category: "Action", id: "98ca4631e85a4ad5a971944ddf3327e4")]
public partial class SwitchLayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<int> Layer;

    protected override Status OnStart()
    {
        if (Self?.Value == null)
        {
            Debug.LogWarning("SwitchLayerAction : Self est null");
            return Status.Failure;
        }

        if (Layer.Value < 0 || Layer.Value > 31)
        {
            Debug.LogWarning($"SwitchLayerAction : index layer '{Layer.Value}' invalide (doit être entre 0 et 31)");
            return Status.Failure;
        }

        Self.Value.layer = Layer.Value;

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}