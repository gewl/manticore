using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumValueComponent : EntityComponent {

    string entityID { get { return entityInformation.Data.ID; } }
    int momentumValue { get { return entityInformation.Data.MomentumValue; } }

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
        GlobalEventEmitter.OnGameStateEvent(GlobalConstants.GameStateEvents.EntityDied, momentumValue.ToString());
    }
}
