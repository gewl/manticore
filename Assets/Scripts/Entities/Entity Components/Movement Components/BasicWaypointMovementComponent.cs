﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicMovementComponent moves at a constant speed in a linear direction to the next waypoint, stopping the entity if the CLEARWAYPOINT event is emitted on the entity.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BasicWaypointMovementComponent : EntityComponent {

    bool isMoving = false;
    float baseMoveSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }
    float currentMoveSpeed;
    bool isOnARamp = false;
    bool isGrounded = false;

    Animator animator;

    protected override void OnEnable()
    {
        base.OnEnable();

        animator = GetComponent<Animator>();
        entityInformation.SetAttribute(EntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
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
            entityInformation.EntityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }
        if (isMoving)
        {
            Vector3 nextWaypointPosition = (Vector3)entityInformation.GetAttribute(EntityAttributes.NextWaypoint);
            Vector3 differenceBetweenPosition = (nextWaypointPosition - entityInformation.transform.position);
            currentMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.CurrentMoveSpeed);
            if (differenceBetweenPosition.magnitude <= 0.2f)
            {
                entityEmitter.EmitEvent(EntityEvents.WaypointReached);
            }
            else
            {
                Vector3 normalizedDifferenceBetweenPosition = differenceBetweenPosition.normalized;
                Vector3 nextVelocity = normalizedDifferenceBetweenPosition * currentMoveSpeed;
                entityInformation.EntityRigidbody.velocity = nextVelocity;
            }
        }
    }

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnMove()
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
        Debug.Log("OnMove");
        isMoving = true;
        currentMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.CurrentMoveSpeed);
    }

    void OnStop()
    {
        Debug.Log("OnStop");
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
        if (isGrounded)
        {
            isMoving = false;
            base.entityInformation.EntityRigidbody.velocity = Vector3.zero;
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
