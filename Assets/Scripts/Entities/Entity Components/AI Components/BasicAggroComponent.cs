﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicAggroComponent checks if the PLAYER enters a serialized aggro range, then transitions the entity to an aggro state.
/// <para>When the player is detected, it EMITs the ONAGGRO event, sets the CURRENTTARGET attribute on EntityData, and activates/deactivates
/// components assigned to two serialized arrays, to change entity behavior between states. </para>
/// </summary>

public class BasicAggroComponent : EntityComponent {
    float _aggroRange;
    float aggroRange
    {
        get
        {
            if (_aggroRange == 0.0f)
            {
                _aggroRange = entityInformation.Data.AggroRange;
            }

            return _aggroRange;
        }
    }

    protected override void Subscribe()
    {
        // This has to happen somewhere, and bundling it into EntityData is a little too prescriptive
        // about what sorts of entities will be composed in this system.
        // This makes the assumption that an entity with the aggro component can be 'aggroed'
        // from its initial state; still, it's a little bizarre.
        entityInformation.SetAttribute(EntityAttributes.IsAggroed, false);

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    public void OnUpdate()
    {
        Vector3 entityPosition = base.entityInformation.EntityTransform.position;
        Vector3 playerPosition = GameManager.GetPlayerPosition();
        float squareDistance = (entityPosition - playerPosition).sqrMagnitude;

        if (squareDistance <= aggroRange * aggroRange)
        {
            SwitchToAggro();
        }
    }

    void SwitchToAggro()
    {
		entityInformation.SetAttribute(EntityAttributes.CurrentTarget, GameManager.GetPlayerTransform());
		entityEmitter.EmitEvent(EntityEvents.TargetUpdated);
		entityInformation.SetAttribute(EntityAttributes.IsAggroed, true);
		entityEmitter.EmitEvent(EntityEvents.Aggro);

        Unsubscribe();
	}
}
