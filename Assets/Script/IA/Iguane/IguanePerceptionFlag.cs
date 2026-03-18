using System;
using Unity.Behavior;

[Flags]
[BlackboardEnum]
public enum IguanePerceptionFlag
{
	PlayerSeen = 1 << 0,
	AppatSeen = 1 << 1,
	EnemySeen = 1 << 2,
	RecieveSound = 1 << 3
}
