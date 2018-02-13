﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotation : EntityComponent {

    Rigidbody entityRigidbody;

    [Header("Body Rotation")]
    [SerializeField]
    bool isSmoothing = true;
    [SerializeField]
    int velocitiesToCache = 5;

    int nextUpdateSlot;
    Vector3[] cachedVelocities;

    [Header("Head Rotation")]
    [SerializeField]
    Transform headBone;
    [SerializeField]
    Transform currentTarget;

    protected override void Awake()
    {
        base.Awake();
        entityRigidbody = GetComponent<Rigidbody>();

        cachedVelocities = new Vector3[velocitiesToCache];

        for (int i = 0; i < velocitiesToCache; i++)
        {
            cachedVelocities[i] = Vector3.zero;
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, StopFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, StopFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.FreezeRotation, StopFunctioning);

        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, UpdateTarget);

        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, BeginFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.ResumeRotation, BeginFunctioning);


        BeginFunctioning();
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, StopFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FreezeRotation, StopFunctioning);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, UpdateTarget);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, BeginFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ResumeRotation, BeginFunctioning);

        StopFunctioning();
    }

    void UpdateTarget()
    {
        Transform newTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);

        currentTarget = newTarget;
    }

    void BeginFunctioning()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.LateUpdate, OnLateUpdate);
    }

    void StopFunctioning()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.LateUpdate, OnLateUpdate);
    }

    void OnFixedUpdate()
    {
        Vector3 currentVelocity = entityRigidbody.velocity;
        if (currentVelocity.sqrMagnitude >= 0.1f)
        {
            if (isSmoothing)
            {
                transform.rotation = Quaternion.LookRotation(SmoothVelocity(currentVelocity));
            }
            else
            {
                Quaternion nextRotation = Quaternion.LookRotation(currentVelocity);
                transform.rotation = nextRotation;
            }
        }
    }

    Vector3 SmoothVelocity(Vector3 mostRecentVelocity)
    {
        cachedVelocities[nextUpdateSlot++] = mostRecentVelocity;

        if (nextUpdateSlot == velocitiesToCache)
        {
            nextUpdateSlot = 0;
        }

        Vector3 averagedVelocity = Vector3.zero;

        for (int i = 0; i < velocitiesToCache; i++)
        {
            averagedVelocity += cachedVelocities[i];
        }

        averagedVelocity.y = 0f;
        return averagedVelocity / velocitiesToCache;
    }

    void OnLateUpdate()
    {
        float angleToTarget = 0f;
        if (currentTarget != null)
        {
            Vector3 currentForward = transform.forward;
            angleToTarget = Vector3.Angle(currentForward, currentTarget.position - transform.position);

            Vector3 cross = Vector3.Cross(currentForward, currentTarget.position - transform.position);

            if (cross.y < 0)
            {
                angleToTarget = -angleToTarget;
            }
            Debug.Log(angleToTarget);
        }

        Vector3 currentRotation = transform.rotation.eulerAngles;
        Debug.Log(transform.rotation.x);
        headBone.rotation = Quaternion.Euler(currentRotation.x - 90f, angleToTarget - 180f, currentRotation.z);
    }
}
