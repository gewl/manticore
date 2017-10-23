using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicMovementComponent moves at a constant speed in a linear direction to the next waypoint, stopping the entity if the CLEARWAYPOINT event is emitted on the entity.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BasicWaypointMovementComponent : EntityComponent {

    [SerializeField]
    bool isMoving = true;
    [SerializeField]
    float baseMoveSpeed;
    [SerializeField]
    float currentMoveSpeed;
    bool isOnARamp = false;
    bool isGrounded = false;

    private void OnEnable()
    {
        entityData.SetSoftAttribute(SoftEntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.SetWaypoint, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.ClearWaypoint, OnStop);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.SetWaypoint, OnMove);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.ClearWaypoint, OnStop);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnRecovered);
    }

    void OnFixedUpdate()
    {
        if (!isOnARamp && !isGrounded)
        {
            entityData.EntityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }
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

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnMove()
    {
        isMoving = true;
        currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
    }

    void OnStop()
    {
        if (isGrounded)
        {
            isMoving = false;
            base.entityData.EntityRigidbody.velocity = Vector3.zero;
            currentMoveSpeed = 0f;
        }
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnRecovered()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            isGrounded = false;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = false;
        }
    }
}
