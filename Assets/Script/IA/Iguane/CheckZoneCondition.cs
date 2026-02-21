using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckZone", story: "[Self] Check [AIZone]", category: "Conditions", id: "53e8873cafff2936b766012b1cde5684")]
public partial class CheckZoneCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<AIZone> AIZone;

    public override bool IsTrue()
    {
        float distance = Vector3.Distance(Self.Value.transform.position, AIZone.Value.transform.position) - AIZone.Value.radius;
        if (distance <= 0)
        {
            return true;
        }
        return false;
    }
    
}
