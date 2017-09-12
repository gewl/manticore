using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCombatAIComponent : EntityComponent {

    bool isAggroed = false;
    bool isFunctioning = false;
    bool isChasing = false;
    [SerializeField]
    float fireCooldown;
    [SerializeField]
    float arcOfFire;
    [SerializeField]
    float attackRange;

    [SerializeField]
    float combatMoveSpeed;
    [SerializeField]
    float minimumMovementPause;
    [SerializeField]
    float maximumMovementPause;
    [SerializeField]
    float minimumDistanceToMove = 15;
    [SerializeField]
    float maximumDistanceToMove = 25;

    float currentFireCooldown;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, BeginFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.Recovered, BeginFunctioning);
        if (isAggroed)
        {
            BeginFunctioning();
        }
        currentFireCooldown = 0f;
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, BeginFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Recovered, BeginFunctioning);
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

        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);

        if (!isChasing && !IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
            isChasing = true;
            Invoke("GenerateAndSetWaypoint", 0.5f);
        }
        else if (isChasing && IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
            isChasing = false;
            Invoke("GenerateAndSetWaypoint", UnityEngine.Random.Range(minimumMovementPause, maximumMovementPause));
        }
    }

    void OnDeaggro()
    {
        StopFunctioning();
        isAggroed = false;
    }

    void OnWaypointReached()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        isChasing = false;

        Invoke("GenerateAndSetWaypoint", UnityEngine.Random.Range(minimumMovementPause, maximumMovementPause));
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

    void GenerateAndSetWaypoint()
    {
        if (isChasing)
        {
            Vector3 nextWaypoint = GenerateChaseMovementPosition();
            entityData.SetSoftAttribute(SoftEntityAttributes.NextWaypoint, nextWaypoint);
        }
        else
        {
            Vector3 nextWaypoint = GenerateCombatMovementPosition();
            entityData.SetSoftAttribute(SoftEntityAttributes.NextWaypoint, nextWaypoint);
        }

        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, combatMoveSpeed);

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }

    Vector3 GenerateCombatMovementPosition()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        Vector3 currentPositionDifference = currentTarget.position - transform.position;

        Vector3 tempWaypoint = currentPositionDifference.normalized;
        tempWaypoint.x = tempWaypoint.x + UnityEngine.Random.Range(-.5f, .5f);
        tempWaypoint.z = tempWaypoint.z + UnityEngine.Random.Range(-.5f, .5f);

        tempWaypoint.x = currentTarget.position.x + (tempWaypoint.x * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.z = currentTarget.position.z + (tempWaypoint.z * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.y = transform.position.y;

        return tempWaypoint;
    }

    Vector3 GenerateChaseMovementPosition()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);

        Vector3 lineToTarget = Vector3.Lerp(currentTarget.position, transform.position, 0.5f);
        lineToTarget.y = 1.5f;

        return lineToTarget;
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
            entityEmitter.SubscribeToEvent(EntityEvents.Hurt, StopFunctioning);
            entityEmitter.SubscribeToEvent(EntityEvents.Dead, StopFunctioning);

            entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);

            GenerateAndSetWaypoint();
        }
    }
     
    void StopFunctioning()
    {
        CancelInvoke();
        if (isFunctioning)
        {
            isFunctioning = false;
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, StopFunctioning);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopFunctioning);

            entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
            entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        }
    }
    #endregion
}