using System;
using UnityEngine;
using UnityEngine.VFX;

public class OnOff : MonoBehaviour, IInteractable
{
    [Header("IInteractable")]
    public int Priority { get; set; } = 1;
    [SerializeField] private GameObject interactionText;

    public bool CanInteract { get; set; } = true;
    
    [SerializeField] private bool startsQuest;
    [SerializeField] private bool updateQuest;
    
    [Header("Noise Settings")]
    [SerializeField] private float NoiseRadius = 10f;
    [SerializeField] private float NoiseOn = 10f;
    [SerializeField] private AudioClip clip;
    [SerializeField] public bool isActive;

    [Header("Effects")]
    [SerializeField] private VisualEffect vfx;

    private MakeNoise _makeNoise;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        _makeNoise = GetComponent<MakeNoise>();
        audioSource = GetComponent<AudioSource>();

        // S'assure que le VFX est éteint au début
        if (vfx != null)
            vfx.Stop();
    }

    public void Update()
    {
        if (isActive)
        {
            // Le stimulus est émis en continu indépendamment du son
            WorldStimulusManager.Instance?.EmitSound(transform.position, NoiseRadius, NoiseOn, gameObject);
            
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();

                // Lance le VFX quand le son démarre
                if (vfx != null)
                    vfx.Play();
            }
        }
    }

    public void Interact(PlayerInteractions interactions)
    {
        isActive = !isActive;

        if (!isActive)
        {
            audioSource.Stop();

            // Stop le VFX quand on éteint
            if (vfx != null)
                vfx.Stop();
        }

        if (startsQuest)
            this.gameObject.GetComponent<Quests.StartQuestScript>().StartQuest();

        if (updateQuest)
            this.gameObject.GetComponent<Quests.UpdateQuest>().UpdateQuestProgress();
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        interactionText.SetActive(false);
    }
}