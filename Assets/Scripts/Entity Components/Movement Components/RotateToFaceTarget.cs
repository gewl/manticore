using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTarget : EntityComponent {

    [SerializeField]
    float rotationStrength = 3f;
    Transform currentTarget;
    Vector3 nextWaypoint;

    protected override void Awake()
    {
        base.Awake();
        nextWaypoint = Vector3.forward;
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.SetWaypoint, OnSetWaypoint);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.SetWaypoint, OnSetWaypoint);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnRecovered);
    }

    void OnSetWaypoint()
    {
        nextWaypoint = (Vector3)entityData.GetAttribute(EntityAttributes.NextWaypoint);
    }

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnTargetUpdated()
    {
        Transform newTarget = (Transform)entityData.GetAttribute(EntityAttributes.CurrentTarget);
        currentTarget = newTarget;
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnRecovered()
    {
        currentTarget = (Transform)entityData.GetAttribute(EntityAttributes.CurrentTarget);
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
        Vector3 lookPosition;
        if (currentTarget != null)
        {
            lookPosition = currentTarget.position;
        }
        else
        {
            lookPosition = nextWaypoint;
        }
        lookPosition.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }
}
