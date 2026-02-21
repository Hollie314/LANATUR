using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LeaveSpot", story: "[Self] leave [Spot] inside [WaypointManager]", category: "Action", id: "9ae5c7abe01103d09e7a767d51b8a3c7")]
public partial class LeaveSpotAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Spot;
    [SerializeReference] public BlackboardVariable<WaypointManager> WaypointManager;

    protected override Status OnStart()
    {
        WaypointManager.Value.waypoints[Spot.Value].Remove(Self.Value);
        return Status.Success;
    }
    
}

