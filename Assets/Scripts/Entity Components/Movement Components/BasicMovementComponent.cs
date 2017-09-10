﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicMovementComponent moves at a constant speed in a linear direction to the next waypoint, stopping the entity if the CLEARWAYPOINT event is emitted on the entity.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BasicMovementComponent : EntityComponent {

    [SerializeField]
    bool isMoving = true;
    [SerializeField]
    float currentMoveSpeed;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.SetWaypoint, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.ClearWaypoint, OnStop);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnUpdate()
    {
        if (isMoving)
        {
            Vector3 nextWaypointPosition = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.NextWaypoint);
            Vector3 differenceBetweenPosition = (nextWaypointPosition - entityData.transform.position);
            currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
            if (differenceBetweenPosition.magnitude <= 0.2f)
            {
                entityEmitter.EmitEvent(EntityEvents.WaypointReached);
            }
            else
            {
                Vector3 normalizedDifferenceBetweenPosition = differenceBetweenPosition.normalized;
                Vector3 nextVelocity = normalizedDifferenceBetweenPosition * currentMoveSpeed;
                entityData.EntityRigidbody.velocity = nextVelocity;
            }
        }
    }

    void OnMove()
    {
        isMoving = true;
        currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
    }

    void OnStop()
    {
        isMoving = false;
        base.entityData.EntityRigidbody.velocity = Vector3.zero;
        currentMoveSpeed = 0f;
    }
}