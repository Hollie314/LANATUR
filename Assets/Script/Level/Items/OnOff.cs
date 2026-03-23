using System;
using UnityEngine;

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

    private MakeNoise _makeNoise;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        _makeNoise = GetComponent<MakeNoise>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isActive)
        {
            if(!audioSource.isPlaying)
                _makeNoise.Noise(transform.position, NoiseRadius, NoiseOn, gameObject, audioSource, clip);
        }
    }

    public void Interact(PlayerInteractions interactions)
    {
        isActive = !isActive;
        if (!isActive)
            audioSource.Stop();
        
        if (startsQuest)
            this.gameObject.GetComponent<Quests.StartQuestScript>().StartQuest();
            
        if (updateQuest)
            this.gameObject.GetComponent<Quests.UpdateQuest>().UpdateQuestProgress();
    }

    public void OnPlayerEnter(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut etre interargie avec");
        interactionText.SetActive(true);
    }

    public void OnPlayerExit(PlayerInteractions interactions)
    {
        Debug.Log("cassette peut plus etre interargie avec");
        interactionText.SetActive(false);
    }
}