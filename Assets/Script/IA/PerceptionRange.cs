using UnityEngine;

public class PerceptionRange : MonoBehaviour, IStimulusListener
{
    public bool Detected = false;

    [Header("Ranges")]
    public float soundRadius = 15f;
    public float lightRadius = 5f;

    [Header("Stimulus")]
    [HideInInspector] public float soundIntensity = 0f;
    public float lightIntensity = 5f;

    [Header("Settings")]
    public float soundDecayRate = 5f;
    public float memoryBias = 1.1f; // évite de changer de cible trop facilement

    public bool showDebugVisuals = true;

    void OnEnable()
    {
        soundIntensity = 0;

        if (WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.RegisterListener(this);
    }

    void OnDisable()
    {
        if (WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.UnregisterListener(this);
    }

    void Update()
    {
        // décroissance progressive du stimulus sonore
        soundIntensity = Mathf.Max(0, soundIntensity - Time.deltaTime * soundDecayRate);
    }

    public void OnSoundReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        // ignore son propre bruit
        if (source == gameObject)
            return;

        float distance = Vector3.Distance(transform.position, position);

        // hors portée du son
        if (distance > radius)
            return;

        // 🔊 atténuation avec distance
        float strength = intensity * (1f - (distance / radius));

        // ignore si moins important que le stimulus actuel
        if (strength < soundIntensity * memoryBias)
            return;

        // ✅ accepte le nouveau stimulus
        soundIntensity = strength;

        Debug.Log($"{gameObject.name} hears {source.name} with strength {strength}");
    }

    public void OnLightReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        // ignore son propre stimulus
        if (source == gameObject)
            return;

        float distance = Vector3.Distance(transform.position, position);

        if (distance > lightRadius)
            return;

        if (intensity < lightIntensity)
            return;

        Detected = true;
    }

    public void StopDetection()
    {
        soundIntensity = 0;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || !enabled) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lightRadius);
    }
}