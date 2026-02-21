using System;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeaSpot", story: "[Self] Take a [Spot] inside [WaytpointManager]", category: "Action", id: "e82095f41772222cd1c3aa3bff2ff2ab")]
public partial class TakeaSpotAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Spot;
    [SerializeReference] public BlackboardVariable<WaypointManager> WaytpointManager;
    protected override Status OnStart()
    {
        GameObject spot = Spot.Value;
        for (int i = 0; i < WaytpointManager.Value.waypoints.Count; i++)
        {
            spot = WaytpointManager.Value.waypoints.Keys.ElementAt(i);
            if (WaytpointManager.Value.waypoints[spot].Count == 1)
            {
                if (WaytpointManager.Value.waypoints[spot][0] == Self.Value)
                {
                    break;
                }
            }

            if (WaytpointManager.Value.waypoints[spot].Count == 0)
            {
                WaytpointManager.Value.waypoints[spot].Add(Self);
                break;
            }

            if (i == WaytpointManager.Value.waypoints.Count - 1)
            {
                return Status.Failure;                
            }
        }

        Spot.ObjectValue = spot;
        return Status.Success;
    }
    
}

