using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
