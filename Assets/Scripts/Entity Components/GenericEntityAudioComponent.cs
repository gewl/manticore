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

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
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

    #endregion
}
