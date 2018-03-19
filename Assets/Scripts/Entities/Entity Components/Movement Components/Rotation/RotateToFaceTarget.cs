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
        entityEmitter.SubscribeToEvent(EntityEvents.Respawning, OnRespawn);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnRecovered);

        entityEmitter.SubscribeToEvent(EntityEvents.FreezeRotation, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.ResumeRotation, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.SetWaypoint, OnSetWaypoint);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Respawning, OnRespawn);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnRecovered);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.FreezeRotation, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ResumeRotation, OnRecovered);
    }

    void OnSetWaypoint()
    {
        nextWaypoint = (Vector3)entityInformation.GetAttribute(EntityAttributes.NextWaypoint);
    }

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnRespawn()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    void OnTargetUpdated()
    {
        Transform newTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        currentTarget = newTarget;
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnRecovered()
    {
        currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
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
            // TODO: Change this to random swiveling, probably?
            lookPosition = nextWaypoint;
        }
        lookPosition.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
        float str = Mathf.Min(rotationStrength * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);
    }
}
