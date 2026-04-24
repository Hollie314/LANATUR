using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;

    private float timeAlive = 0f;

    void Update()
    {
        timeAlive += Time.deltaTime;
        if(timeAlive >= lifeTime){Destroy(this.gameObject);}
    }
}
