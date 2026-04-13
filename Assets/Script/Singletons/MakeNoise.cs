using UnityEngine;

public class MakeNoise : MonoBehaviour
{
    public void Noise(
        Vector3 position,
        float radius,
        float intensity,
        GameObject source,
        AudioSource audioSource = null,
        AudioClip audioClip = null)
    {
        // jouer son si fourni
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // envoyer stimulus au monde
        WorldStimulusManager.Instance.EmitSound(
            position,
            radius,
            intensity,
            source
        );

        //Debug.Log("Noise emitted at " + position);
    }
    
}