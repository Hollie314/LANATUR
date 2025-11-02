using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShoutAtTarget : MonoBehaviour
{
    public float shoutForce = 5f;     // Force de recul appliquée au joueur
    public float shoutCooldown = 3f;  // Temps entre deux cris
    public AudioClip shoutSound;      // Optionnel, son du cri

    private float lastShoutTime;
    private AudioSource audioSource;
    private TargetDetectionFilter filter;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        filter = GetComponent<TargetDetectionFilter>();
    }

    public bool CanShout => Time.time >= lastShoutTime + shoutCooldown;

    public bool TryShout()
    {
        if (filter == null || filter.currentTarget == null || !CanShout)
            return false;

        // 💥 Cri visuel ou sonore
        Debug.Log($"{name} crie sur {filter.currentTarget.name} !");
        if (shoutSound != null)
            audioSource.PlayOneShot(shoutSound);

        // 🔁 Effet sur la cible (recul, stun, etc.)
        Rigidbody rb = filter.currentTarget.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (filter.currentTarget.position - transform.position).normalized;
            rb.AddForce(dir * shoutForce, ForceMode.Impulse);
        }

        lastShoutTime = Time.time;
        return true;
    }
}