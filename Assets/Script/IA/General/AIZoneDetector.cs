using UnityEngine;

public class AIZoneDetector : MonoBehaviour
{
    public AIBrainMemory memory;

    void OnTriggerEnter(Collider other)
    {
        AIZone zone = other.GetComponent<AIZone>();

        if (zone == null) return;

        switch(zone.zoneType)
        {
            case AIZone.ZoneType.Territory:
                memory.territoryZones.Add(zone);
                break;

            case AIZone.ZoneType.Food:
                memory.foodZones.Add(zone);
                break;

            case AIZone.ZoneType.Patrol:
                memory.patrolZones.Add(zone);
                break;

            case AIZone.ZoneType.Safe:
                memory.safeZones.Add(zone);
                break;
        }
    }
}