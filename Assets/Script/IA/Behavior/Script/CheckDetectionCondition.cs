using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "CheckDetection",
    story: "Check [Target] in [RangeDetection] at layer [LayerIndex]",
    category: "Conditions",
    id: "549a071199e4f57675a1ab1faee1bb27")]
public partial class CheckDetectionCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;        // juste un GameObject
    [SerializeReference] public BlackboardVariable<RangeDetection> RangeDetection;
    [SerializeReference] public BlackboardVariable<int> LayerIndex;           // index du layer à checker

    public override bool IsTrue()
    {
        if (RangeDetection.Value == null)
            return false;

        // Récupère le premier target depuis RangeDetection pour le layer choisi
        Target.Value = RangeDetection.Value.UpdateDetector(LayerIndex.Value);

        // Condition true si un target est présent
        return Target.Value != null;
    }
}