using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTarget : EntityComponent {

    float _rotationStrength;
    float RotationStrength
    {
        get
        {
            if (_rotationStrength == 0.0f)
            {
                _rotationStrength = entityInformation.Data.AggroRange;
            }

            return _rotationStrength;
        }
    }

    Transform headObject;
    Transform currentTarget;
    Vector3 nextWaypoint;

    protected override void Awake()
    {
        base.Awake();
        nextWaypoint = Vector3.forward;
        headObject = transform.Find("Firer");

        if (headObject == null)
        {
            headObject = transform;
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.SubscribeToEvent(EntityEvents.LateUpdate, Rotate);
        entityEmitter.SubscribeToEvent(EntityEvents.SetWaypoint, OnSetWaypoint);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnRecovered);

        entityEmitter.SubscribeToEvent(EntityEvents.FreezeRotation, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.ResumeRotation, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.SetWaypoint, OnSetWaypoint);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.LateUpdate, Rotate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnRecovered);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.FreezeRotation, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ResumeRotation, OnRecovered);
    }

    void OnSetWaypoint()
    {
        nextWaypoint = (Vector3)entityInformation.GetAttribute(EntityAttributes.NextWaypoint);
    }


    void OnTargetUpdated()
    {
        Transform newTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        currentTarget = newTarget;
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.LateUpdate, Rotate);
    }

    void OnRecovered()
    {
        currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        if (currentTarget != null)
        {
            entityEmitter.SubscribeToEvent(EntityEvents.LateUpdate, Rotate);
        }
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
        lookPosition.y = headObject.transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
        float str = Mathf.Min(RotationStrength * Time.deltaTime, 1);
        headObject.transform.rotation = Quaternion.Lerp(headObject.transform.rotation, targetRotation, str);
    }
}
