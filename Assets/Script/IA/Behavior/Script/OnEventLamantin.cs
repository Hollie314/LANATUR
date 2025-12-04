using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/On Event Lamantin")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "On Event Lamantin", message: "On Event Lamantin", category: "Events", id: "71361faad4b29f4c866503b943a5c9fd")]
public sealed partial class OnEventLamantin : EventChannel { }

