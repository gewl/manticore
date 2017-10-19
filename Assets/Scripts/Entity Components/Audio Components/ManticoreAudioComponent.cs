using UnityEngine;

public class ManticoreAudioComponent : EntityComponent
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip parrySuccessfulClip;
    [SerializeField]
    AudioClip hurtClip;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.ParrySuccessful, OnParrySuccessful);
        entityEmitter.SubscribeToEvent(EntityEvents.Hurt, OnHurt);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ParrySuccessful, OnParrySuccessful);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, OnHurt);
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
