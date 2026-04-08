using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(MakeNoise))]
[RequireComponent(typeof(AudioSource))]
public class AlarmNoiseEmitter : MonoBehaviour
{
    [Header("Alarm Settings")]
    public bool alarmActive = false;

    [Tooltip("Distance à laquelle le lamantin entend")]
    public float radius = 50f;

    [Tooltip("Force du bruit")]
    public float intensity = 12f;

    [Tooltip("Interval entre émissions")]
    public float emitInterval = 0.5f;

    [Header("Audio")]

    private MakeNoise makeNoise;
    public AudioSource audioSource;
    [SerializeField] private VisualEffect vfx;
    private float timer;

    void Awake()
    {
        makeNoise = GetComponent<MakeNoise>();
        audioSource = GetComponent<AudioSource>();
        //vfx?.Play();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!alarmActive)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EmitNoise();
            timer = emitInterval;
        }
    }

    void EmitNoise()
    {
        makeNoise.Noise(
            transform.position,
            radius,
            intensity,
            gameObject,
            audioSource
        );

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    // ============================
    // CONTROL FUNCTIONS
    // ============================

    public void ActivateAlarm()
    {
        alarmActive = true;
        timer = 0f;
        audioSource.Play();

        Debug.Log("Alarm activated");
    }

    public void StopAlarm()
    {
        alarmActive = false;
        audioSource.Stop();

        Debug.Log("Alarm stopped");
    }
}