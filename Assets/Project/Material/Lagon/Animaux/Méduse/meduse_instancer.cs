using UnityEngine;

public class meduse_instancer : MonoBehaviour
{
    float OffsetTime(float time, float firstTime = 0)
    {
        if (firstTime == 0)
        {
            firstTime = time;
        }

        return firstTime;
    }
}
