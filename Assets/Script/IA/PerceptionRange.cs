using UnityEngine;

public class PerceptionRange : MonoBehaviour, IStimulusListener
{
    public bool Detected = false;
    public float soundRadius = 5f;
    public float lightRadius = 5f;
    public float soundIntensity;
    public float lightIntensity = 5f;
    public bool showDebugVisuals = true;
    
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
        
        float distance = Vector3.Distance(transform.position, position) - radius - soundRadius;
        
        if(distance > 0) 
            return;
        
        soundIntensity = intensity;
        
    }

    public void OnLightReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        // IGNORE SON PROPRE BRUIT
        if(source == gameObject)
            return;
        
        float distance = Vector3.Distance(transform.position, position);
        
        if (intensity > lightIntensity)
            return;

        if(distance > lightRadius) 
            return;
        
        Detected = true;
        
    }
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || this.enabled == false) return;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
        Gizmos.DrawWireSphere(transform.position, lightRadius);

    }

    public void StopDetection()
    {
        soundIntensity = 0;
    }
}
