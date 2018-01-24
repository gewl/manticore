using UnityEngine;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class ManticoreAudioComponent : EntityComponent
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip parryClip;
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
        entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry);
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

    void OnParry()
    {
        if (parryClip != null)
        {
            PlayGearSound(HardwareTypes.Parry);
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
