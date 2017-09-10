using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicAggroComponent checks if the PLAYER enters a serialized aggro range, then transitions the entity to an aggro state.
/// <para>When the player is detected, it EMITs the ONAGGRO event, sets the CURRENTTARGET attribute on EntityData, and activates/deactivates
/// components assigned to two serialized arrays, to change entity behavior between states. </para>
/// </summary>

public class BasicAggroComponent : EntityComponent {
    [SerializeField]
    float aggroRange = 0f;

    [SerializeField]
    EntityComponent[] disableOnAggroComponents;
    [SerializeField]
    EntityComponent[] enableOnAggroComponents;

    public override void Initialize()
    {
        base.entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    public override void Cleanup()
    {
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    public void OnUpdate()
    {
        Vector3 entityPosition = base.entityData.EntityTransform.position;
        Vector3 playerPosition = GameManager.Instance.GetPlayerPosition();
        float squareDistance = (entityPosition - playerPosition).sqrMagnitude;

        if (squareDistance <= aggroRange * aggroRange)
        {
            base.entityData.SetSoftAttribute(SoftEntityAttributes.CurrentTarget, GameManager.Instance.GetPlayerTransform());
            base.entityEmitter.EmitEvent(EntityEvents.Aggro);

            for (int i = 0; i < disableOnAggroComponents.Length; i++)
            {
                EntityComponent component = disableOnAggroComponents[i];
                component.enabled = false;
            }

            for (int i = 0; i < enableOnAggroComponents.Length; i++)
            {
                EntityComponent component = enableOnAggroComponents[i];
                component.enabled = true;
            }
        }
    }
}
