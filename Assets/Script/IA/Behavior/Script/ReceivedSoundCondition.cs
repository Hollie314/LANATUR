using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ReceivedSound", story: "Check Perception [Range] > to [Intensity]", category: "Conditions", id: "7e7658099db0406fa3a6ff9ba1ebff3a")]
public partial class ReceivedSoundCondition : Condition
{
    [SerializeReference] public BlackboardVariable<PerceptionRange> Range;
    [SerializeReference] public BlackboardVariable<float> Intensity;

    public override bool IsTrue()
    {
        if (Range.Value == null)
            return false;

        ////Debug.Log($"hears {Range.Value.soundIntensity}");

        return Range.Value.soundIntensity >= Intensity.Value;
    }
}