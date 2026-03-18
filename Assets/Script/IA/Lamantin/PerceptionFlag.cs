using System;
using Unity.Behavior;

[Flags]
[BlackboardEnum]
public enum PerceptionFlag
{
	PlayerSeen = 1 << 0,
	HeardNoise = 1 << 1,
	WallBlocking = 1 << 2
}
