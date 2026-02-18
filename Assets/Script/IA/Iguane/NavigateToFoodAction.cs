using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavigateToFood", story: "Take random [Target] inside [FoodList]", category: "Action", id: "5ab1fa8c30711ceeb696f2bf7b95e30c")]
public partial class NavigateToFoodAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<List<GameObject>> FoodList;

    protected override Status OnStart()
    {
        if (FoodList.Value == null || FoodList.Value.Count == 0)
            return Status.Failure;

        Target.ObjectValue = FoodList.Value[Random.Range(0, FoodList.Value.Count)];
        return Status.Success;
    }
}

