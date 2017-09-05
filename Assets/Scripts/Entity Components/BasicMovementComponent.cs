using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BasicMovementComponent : EntityComponent {

    [SerializeField]
    bool isMoving = true;
    [SerializeField]
    float baseMoveSpeed;
    float currentMoveSpeed;

    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        base.entityEmitter.SubscribeToEvent(EntityEvents.Move, OnMove);
        base.entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
        currentMoveSpeed = baseMoveSpeed;

        base.entityData.Expect(SoftEntityAttributes.CurrentMoveSpeed);
    }

    public override void Cleanup()
    {
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnUpdate()
    {
        if (isMoving)
        {
            Vector3 nextWaypointPosition = (Vector3)base.entityData.GetSoftAttribute(SoftEntityAttributes.NextWaypoint);
            Vector3 differenceBetweenPosition = (nextWaypointPosition - base.entityData.transform.position);
            if (differenceBetweenPosition.magnitude <= 0.1f)
            {
                base.entityEmitter.EmitEvent(EntityEvents.WaypointReached);
            }
            else
            {
                Vector3 normalizedDifferenceBetweenPosition = differenceBetweenPosition.normalized;
                Vector3 nextVelocity = normalizedDifferenceBetweenPosition * currentMoveSpeed;
                base.entityData.EntityRigidbody.velocity = nextVelocity;
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
