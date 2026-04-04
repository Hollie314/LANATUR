using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(AudioSource))]
public class ShoutAtTarget : MonoBehaviour
{
    [SerializeField] private GameObject ShoutSphere;

    [SerializeField] private float shoutFinalRadius;
    public bool isShouting { get; private set; }
    private float ShoutingTime;
    private float timePassed;
    private GameObject invokedSphere;
    

    private void Update()
    {
        if (isShouting)
        {
            timePassed += Time.deltaTime;
            invokedSphere.transform.localScale = Vector3.one * Mathf.Lerp(0, ShoutingTime, timePassed) * shoutFinalRadius;
            Debug.Log($"Sphere Scale: {invokedSphere.transform.localScale}");
            if (timePassed >= ShoutingTime)
            {
                timePassed = 0;
                isShouting = false;
                Destroy(invokedSphere);
                Debug.Log("Sphere Destroyed");
            }
        }
    }

    public void TryShout(float shoutTime, GameObject Iguane)
    {
        invokedSphere = Instantiate(ShoutSphere, Iguane.transform.position, Quaternion.identity);
        isShouting = true;
        ShoutingTime = shoutTime;
    }
}