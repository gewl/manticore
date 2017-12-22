using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumValueComponent : EntityComponent {

    [SerializeField]
    int momentumValue;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
    }

    private void OnDead()
    {
        if (momentumValue == 0)
        {
            Debug.LogError("Momentum value is zero.");
        }
        MomentumManager.AddMomentum(momentumValue);
    }
}
