using UnityEngine;

public interface IStimulusListener
{
    void OnSoundReceived(Vector3 sourcePos, float radius, float intensity, GameObject source);
    void OnLightReceived(Vector3 sourcePos, float radius, float intensity, GameObject source);
}