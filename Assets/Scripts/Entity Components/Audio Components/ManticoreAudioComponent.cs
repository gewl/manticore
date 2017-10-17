using UnityEngine;

public class ManticoreAudioComponent : EntityComponent
{
    AudioSource audioSource;

    [SerializeField]
    AudioClip parrySuccessfulClip;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.ParrySuccessful, OnParrySuccessful);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ParrySuccessful, OnParrySuccessful);
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

    #endregion
}
