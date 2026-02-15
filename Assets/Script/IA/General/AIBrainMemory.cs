using System.Collections.Generic;
using UnityEngine;

public class AIBrainMemory : MonoBehaviour
{
    public List<AIZone> territoryZones = new();
    public List<AIZone> foodZones = new();
    public List<AIZone> patrolZones = new();
    public List<AIZone> safeZones = new();
    public List<AIZone> lightZones = new();

    public AIZone GetClosestZone(List<AIZone> zones)
    {
        AIZone closest = null;
        float minDist = Mathf.Infinity;

        foreach (var zone in zones)
        {
            float dist = Vector3.Distance(transform.position, zone.GetPoint());

            if (dist < minDist)
            {
                minDist = dist;
                closest = zone;
            }
        }

        return closest;
    }
}