using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateToFood", story: "Take random [GameObject] inside [list]", category: "Action", id: "5ab1fa8c30711ceeb696f2bf7b95e30c")]
public partial class NavigateToFoodAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> GameObject;
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;

    protected override Status OnStart()
    {
        GameObject.Value = List.Value[Random.Range(0, List.Value.Count)];
        return Status.Success;
    }
    
}

