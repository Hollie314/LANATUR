using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MakeNoise", story: "[Self] make [noise] [radius] Raduis & [intensity] Intensity & [audiosource] play [Sound]", category: "Action", id: "00d6bc9aafb981a7d1745e11b4278afe")]
public partial class MakeNoiseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<MakeNoise> Noise;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> Intensity;
    [SerializeReference] public BlackboardVariable<AudioSource> Audiosource;
    [SerializeReference] public BlackboardVariable<AudioClip> Sound;
    protected override Status OnStart()
    {
        Noise.Value.Noise(Self.Value.transform.position, Radius.Value, Intensity.Value, Self.Value,Audiosource.Value, Sound.Value);
        return Status.Success;
    }

   
}

