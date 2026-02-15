using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Waypoints with Pathee", story: "Set [waypoints] with [Pathee]", category: "Action", id: "b1f158f68a94323b416881a9d51bc58e")]
public partial class SetWaypointsWithPatheeAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    //[SerializeReference] public BlackboardVariable<Pathee> Pathee;

    protected override Status OnStart()
    {
        List<GameObject> NewWaypoints = new List<GameObject>();
        //foreach (Transform transform in Pathee.Value.waypoints)
        {
          //  NewWaypoints.Add(transform.gameObject);
        }

        Waypoints.Value = NewWaypoints;
        return Status.Success;
    }
}

