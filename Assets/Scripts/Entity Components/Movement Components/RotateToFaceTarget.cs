using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTarget : EntityComponent {

    [SerializeField]
    float rotationStrength;
    Transform currentTarget;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnRecovered);
        if (currentTarget != null)
        {
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        }
    }

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnTargetUpdated()
    {
        Transform newTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);

        if (newTarget != null)
        {
            entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        }
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnRecovered()
    {
        currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        if (currentTarget != null)
        {
            entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        }
    }

    void OnUpdate()
    {
        Rotate();
    }

    void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.position - transform.position);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }
}
