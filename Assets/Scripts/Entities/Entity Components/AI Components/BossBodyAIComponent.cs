using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBodyAIComponent : EntityComponent
{
    RangedEntityData _entityData;
    RangedEntityData EntityData
    {
        get
        {
            if (_entityData == null)
            {
                _entityData = entityInformation.Data as RangedEntityData;
            }

            return _entityData;
        }
    }

    float FireCooldown { get { return EntityData.AttackCooldown; } }

    float timeElapsedSinceLastFire;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, Connect);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, Connect);
        
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, Disconnect);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, Connect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, Connect);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, Disconnect);
    }

    void Connect()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    void Disconnect()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnUpdate()
    {
        if (timeElapsedSinceLastFire < FireCooldown)
        {
            timeElapsedSinceLastFire += Time.deltaTime;
        }
        else
        {
            entityEmitter.EmitEvent(EntityEvents.PrimaryFire);
            timeElapsedSinceLastFire = 0.0f;
        }
    }

}