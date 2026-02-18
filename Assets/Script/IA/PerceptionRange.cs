using UnityEngine;

public class PerceptionRange : MonoBehaviour, IStimulusListener
{
    public bool Detected = false;
    public float radius = 5f;
    public float intensity = 5f;
    [SerializeField] private bool showDebugVisuals = true;
    
    void OnEnable()
    {
        Debug.Log($"Instance exist {WorldStimulusManager.Instance != null}");
        if(WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.RegisterListener(this);
    }

    void OnDisable()
    {
        if(WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.UnregisterListener(this);
    }
    public void OnSoundReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        Debug.Log("j'entend");
        // IGNORE SON PROPRE BRUIT
        if(source == gameObject)
            return;
        
        float distance = Vector3.Distance(transform.position, position);

        if(distance > radius) return;
        
        Detected = true;
    }

    public void OnLightReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        // IGNORE SON PROPRE BRUIT
        if(source == gameObject)
            return;
        
        float distance = Vector3.Distance(transform.position, position);

        if(distance > radius) return;
        
        Detected = true;
        
    }
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || this.enabled == false) return;
        Gizmos.DrawWireSphere(transform.position, radius);

    }

    public void StopDetection()
    {
        Detected = false;
    }
}
