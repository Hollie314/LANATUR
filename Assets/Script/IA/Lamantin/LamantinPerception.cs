using UnityEngine;

public class LamantinPerception : MonoBehaviour, IStimulusListener
{
    public AI_Manatee manatee;

    void Awake()
    {
        if(manatee == null)
            manatee = GetComponent<AI_Manatee>();
    }

    void OnEnable()
    {
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
        // IGNORE SON PROPRE BRUIT
        if(source == gameObject)
            return;

        Debug.Log("Lamantin heard external sound");

        float distance = Vector3.Distance(transform.position, position);

        if(distance > radius)
            return;

        if(intensity >= manatee.panicThreshold)
            manatee.OnPanicked(position, intensity);

        else if(intensity >= manatee.agitationThreshold)
            manatee.OnAgitated(position, intensity);
    }

    public void OnLightReceived(Vector3 position, float intensity, float radius, GameObject source)
    {
        float distance = Vector3.Distance(transform.position, position);

        if(distance > radius) return;

        if(intensity >= manatee.lightAttractionThreshold)
            manatee.OnLightAttracted(position, intensity);
    }
}