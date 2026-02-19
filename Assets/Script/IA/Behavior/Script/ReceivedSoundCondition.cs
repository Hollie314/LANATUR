using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ReceivedSound", story: "Check Perception [Range]", category: "Conditions", id: "7e7658099db0406fa3a6ff9ba1ebff3a")]
public partial class ReceivedSoundCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PerceptionRange> Range;

    public override bool IsTrue()
    {
        return Range.Value.Detected ;
    }
    
}
