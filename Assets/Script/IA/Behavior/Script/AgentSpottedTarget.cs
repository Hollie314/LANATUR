using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Agent spotted Target")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Agent spotted Target", message: "[Agent] has spotted [Target]", category: "Events", id: "0ca92a96b193a693de16aee47da440df")]
public sealed partial class AgentSpottedTarget : EventChannel<GameObject, GameObject> { }

