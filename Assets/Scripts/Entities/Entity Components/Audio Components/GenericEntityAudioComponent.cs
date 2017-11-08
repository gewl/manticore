using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An audio component that supports playing sounds on a few (supposedly universal) entity events.
/// <para>Namely: Fire, aggro, take damage, die.</para>
/// </summary>

public class GenericEntityAudioComponent : EntityComponent {

    AudioSource audioSource;

    [SerializeField]
    AudioClip primaryFireClip;
    [SerializeField]
    AudioClip deadClip;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
    }

    #region event listeners

    void OnPrimaryFire()
    {
        if (primaryFireClip != null)
        {
            audioSource.clip = primaryFireClip;
            audioSource.Play();
        }
    }

    void OnDead()
    {
        if (deadClip != null)
        {
            audioSource.clip = deadClip;
            audioSource.Play();
        }
    }

    #endregion
}
