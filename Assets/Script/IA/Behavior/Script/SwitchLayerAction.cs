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
            return Status.Failure;

        if (Layer.Value < 0 || Layer.Value > 31)
            return Status.Failure;

        SetLayerRecursively(Self.Value, Layer.Value);

        return Status.Success;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}