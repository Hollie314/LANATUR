using UnityEngine;

public class IguanePerception : MonoBehaviour, IStimulusListener
{
    public AI_Iguane iguane;

    void Awake()
    {
        if (iguane == null)
            iguane = GetComponent<AI_Iguane>();
    }

    void OnEnable()
    {
        if (WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.RegisterListener(this);
    }

    void OnDisable()
    {
        if (WorldStimulusManager.Instance != null)
            WorldStimulusManager.Instance.UnregisterListener(this);
    }

    public void OnSoundReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        if (source == gameObject) return;

        iguane.OnNoise(position, intensity);
    }

    public void OnLightReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        // Pas de réaction à la lumière
    }
}