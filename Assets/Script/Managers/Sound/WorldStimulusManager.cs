using System.Collections.Generic;
using UnityEngine;

public class WorldStimulusManager : MonoBehaviour
{
    public static WorldStimulusManager Instance;

    private List<IStimulusListener> listeners = new List<IStimulusListener>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterListener(IStimulusListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(IStimulusListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    public void EmitSound(Vector3 position, float radius, float intensity, GameObject source)
    {
        foreach (var listener in listeners)
        {
            // ORDRE CORRECT
            listener.OnSoundReceived(position, intensity, radius, source);
        }
    }

    public void EmitLight(Vector3 position, float radius, float intensity, GameObject source)
    {
        foreach (var listener in listeners)
        {
            // ORDRE CORRECT
            listener.OnLightReceived(position, intensity, radius, source);
        }
    }
}