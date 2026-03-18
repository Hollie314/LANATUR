using System;
using Unity.Behavior;

[Flags]
[BlackboardEnum]
public enum IguanePerceptionFlag
{
	None = 0,
	PlayerSeen = 1,
	HeardNoise = 2,
	EnemySeen = 4,
	AppatSeen = 8
}
