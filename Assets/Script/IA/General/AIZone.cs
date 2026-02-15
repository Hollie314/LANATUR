using UnityEngine;

public class AIZone : MonoBehaviour
{
    public enum ZoneType
    {
        Territory,
        Nest,
        Food,
        Patrol,
        Light,
        Noise,
        Safe
    }

    public ZoneType zoneType;

    [Header("Importance")]
    public float priority = 1f;

    [Header("Optional settings")]
    public Transform centerPoint;
    public float radius = 10f;
    

    public Vector3 GetPoint()
    {
        if (centerPoint != null)
            return centerPoint.position;

        return transform.position;
       
    }
}