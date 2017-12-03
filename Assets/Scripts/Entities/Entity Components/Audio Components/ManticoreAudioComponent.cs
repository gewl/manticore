using UnityEngine;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class ManticoreAudioComponent : EntityComponent
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip parrySuccessfulClip;
    [SerializeField]
    AudioClip hurtClip;
    [SerializeField]
    Dictionary<HardwareTypes, AudioClip> activeHardwareSounds;
    
    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Hurt, OnHurt);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, OnHurt);
    }

    public void PlayGearSound(HardwareTypes gear)
    {
        AudioClip gearClip = activeHardwareSounds[gear];
        audioSource.clip = gearClip;
        audioSource.Play();
    }

    public void PlayOutsideSound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    #region event listeners

    void OnParrySuccessful()
    {
        if (parrySuccessfulClip != null)
        {
            audioSource.clip = parrySuccessfulClip;
            audioSource.Play();
        }
    }

    void OnHurt()
    {
        if (hurtClip != null)
        {
            audioSource.clip = hurtClip;
            audioSource.Play();
        }
    }

    #endregion
}
