using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class RangeDetector : MonoBehaviour
{
    public List<GameObject> GameObjectsDetected;

    public void ChangeObjectsDetected(GameObject newObj, bool add, bool crouchTrigger, bool charCrouched)
    {
        if (add)
        {
            if (!GameObjectsDetected.Contains(newObj))
            {
                GameObjectsDetected.Add(newObj);
            }
        }
        else
        {
            if(crouchTrigger)
            {
                if(charCrouched)
                {
                    GameObjectsDetected.Remove(newObj);
                    return;
                }
                return;
            }
            GameObjectsDetected.Remove(newObj);
            return;

        }
    }
}