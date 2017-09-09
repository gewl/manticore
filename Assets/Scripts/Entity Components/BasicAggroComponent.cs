using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAggroComponent : EntityComponent {
    [SerializeField]
    float aggroRange = 0f;

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
            base.entityEmitter.EmitEvent(EntityEvents.Aggro);
        }
    }
}
