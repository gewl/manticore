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
    [SerializeField]
    AudioClip aggroClip;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
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

    void OnAggro()
    {
        if (aggroClip != null)
        {
            audioSource.clip = aggroClip;
            audioSource.Play();
        }
    }

    #endregion
}
