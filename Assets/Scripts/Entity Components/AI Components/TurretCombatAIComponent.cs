using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCombatAIComponent : EntityComponent
{

    bool isAggroed = false;
    bool isFunctioning = false;
    [SerializeField]
    float fireCooldown;
    [SerializeField]
    float arcOfFire;
    [SerializeField]
    float attackRange;

    float currentFireCooldown;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, BeginFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, BeginFunctioning);
        if (isAggroed)
        {
            BeginFunctioning();
        }
        currentFireCooldown = 0f;
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, BeginFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, BeginFunctioning);
        if (isAggroed)
        {
            StopFunctioning();
        }
    }

    #region EntityEvent handlers

    void OnUpdate()
    {
        if (currentFireCooldown > 0f)
        {
            currentFireCooldown -= Time.deltaTime;
        }
        else
        {
            TryToFirePrimary();
        }
    }

    void OnDeaggro()
    {
        StopFunctioning();
        isAggroed = false;
    }

    #endregion

    #region discrete functions to offload event listeners

    void TryToFirePrimary()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        Vector3 directionToTarget = currentTarget.position - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (Mathf.Abs(angleToTarget) <= arcOfFire && IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.PrimaryFire);
            currentFireCooldown = fireCooldown;
        }
    }


    bool IsInRange(Transform target)
    {
        float squaredDistanceToTarget = (target.position - transform.position).sqrMagnitude;

        if (squaredDistanceToTarget >= attackRange * attackRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

    #region Reused subscription bundles
    void BeginFunctioning()
    {
        if (!isAggroed)
        {
            isAggroed = true;
        }
        if (!isFunctioning)
        {
            isFunctioning = true;
            entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
            entityEmitter.SubscribeToEvent(EntityEvents.Deaggro, OnDeaggro);
            entityEmitter.SubscribeToEvent(EntityEvents.Stun, StopFunctioning);
            entityEmitter.SubscribeToEvent(EntityEvents.Dead, StopFunctioning);
        }
    }

    void StopFunctioning()
    {
        if (isFunctioning)
        {
            isFunctioning = false;
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, StopFunctioning);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopFunctioning);
        }
    }
    #endregion
}