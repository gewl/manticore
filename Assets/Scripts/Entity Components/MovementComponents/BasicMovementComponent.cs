using System;
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
    float baseMoveSpeed;
    float currentMoveSpeed;

    public override void Initialize()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.SetWaypoint, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.ClearWaypoint, OnStop);
        currentMoveSpeed = baseMoveSpeed;

        entityData.Expect(SoftEntityAttributes.CurrentMoveSpeed);
    }

    public override void Cleanup()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnUpdate()
    {
        if (isMoving)
        {
            Vector3 nextWaypointPosition = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.NextWaypoint);
            Vector3 differenceBetweenPosition = (nextWaypointPosition - entityData.transform.position);
            if (differenceBetweenPosition.magnitude <= 0.1f)
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
        currentMoveSpeed = baseMoveSpeed;
    }

    void OnStop()
    {
        isMoving = false;
        base.entityData.EntityRigidbody.velocity = Vector3.zero;
        currentMoveSpeed = 0f;
    }
}
