using UnityEngine;

public class StimulusEmitterSound : MonoBehaviour
{
    public float radius = 15f;
    public float intensity = 20f;

    public void Emit()
    {
        if (WorldStimulusManager.Instance != null)
        {
            WorldStimulusManager.Instance.EmitSound(
                transform.position,
                radius,
                intensity,
                gameObject
            );
        }
    }
}