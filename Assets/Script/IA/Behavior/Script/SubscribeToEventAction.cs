using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Subscribe to event", story: "subscribe to [event]", category: "Action", id: "d566c589f07069eb8cd2c9a01be934a6")]
public partial class SubscribeToEventAction : Action
{
    [SerializeReference] public BlackboardVariable<StoryEvent> Event;

    protected override Status OnUpdate()
    {
        if(Event.Value.isEventActive)
            return Status.Success;
        return Status.Running;
    }
}

