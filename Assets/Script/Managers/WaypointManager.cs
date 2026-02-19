using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class WaypointManager : SerializedMonoBehaviour
{
    public Dictionary<GameObject, List<GameObject>> waypoints = new Dictionary<GameObject, List<GameObject>>();
    
}
