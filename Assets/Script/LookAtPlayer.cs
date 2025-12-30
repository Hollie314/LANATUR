using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, new Vector3(0,0,0));
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
    }
}
