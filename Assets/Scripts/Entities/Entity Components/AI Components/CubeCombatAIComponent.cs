using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCombatAIComponent : EntityComponent {

    bool isAggroed = false;
    bool isFunctioning = false;
    bool isChasing = false;

    RangedEntityData _entityData;
    RangedEntityData entityData
    {
        get
        {
            if (_entityData == null)
            {
                _entityData = entityInformation.Data as RangedEntityData;
            }

            return _entityData;
        }
    }

    float fireCooldown { get { return entityData.FireCooldown; } }
    float arcOfFire { get { return entityData.ArcOfFire; } }
    float attackRange { get { return entityData.AttackRange; } }

    [SerializeField]
    float combatMoveSpeedModifier = 1f;
    [SerializeField]
    float chaseMoveSpeedModifier = 1f;
    [SerializeField]
    float minimumMovementPause = 1.0f;
    [SerializeField]
    float maximumMovementPause = 1.5f;

    float timeElapsedSinceLastFire;
    Transform currentTarget;

    [SerializeField]
    AnimationCurve gunHeatCurve;

    const string FIRER_ID = "Firer";
    Transform firer;
    Renderer firerRenderer;
    Color firerOriginalSkin;

    protected override void Awake()
    {
        base.Awake();

        firer = transform.FindChildByRecursive(FIRER_ID);
        if (firer == null)
        {
            Debug.LogError("Firer not found in " + gameObject.name);
        }
        firerRenderer = firer.GetComponent<Renderer>();
        firerOriginalSkin = firerRenderer.material.color;
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, BeginFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, BeginFunctioning);
        if (isAggroed)
        {
            BeginFunctioning();
        }
        timeElapsedSinceLastFire = 0f;
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
        if (timeElapsedSinceLastFire < fireCooldown)
        {
            timeElapsedSinceLastFire += Time.deltaTime;

            float percentageComplete = timeElapsedSinceLastFire / fireCooldown;
            firerRenderer.material.color = Color.Lerp(firerOriginalSkin, Color.red, gunHeatCurve.Evaluate(percentageComplete));
        }
        else
        {
            TryToFirePrimary();
            timeElapsedSinceLastFire = 0f;
            firerRenderer.material.color = firerOriginalSkin;
        }

        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);

        if (!isChasing && !IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
            isChasing = true;
            Invoke("GenerateAndSetWaypoint", UnityEngine.Random.Range(minimumMovementPause, maximumMovementPause));
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
        Debug.Log("Wayponit reached");
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        isChasing = false;

        Invoke("GenerateAndSetWaypoint", UnityEngine.Random.Range(minimumMovementPause, maximumMovementPause));
    }

    #endregion

    #region discrete functions to offload event listeners

    void TryToFirePrimary()
    {
        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        Vector3 directionToTarget = currentTarget.position - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (Mathf.Abs(angleToTarget) <= arcOfFire && IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.PrimaryFire);
            timeElapsedSinceLastFire = fireCooldown; 
        }
    }

    void GenerateAndSetWaypoint()
    {
        currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);

        //if (Mathf.Abs(currentTarget.position.y - transform.position.y) > 1f)
        //{
        //    Vector3 nextWaypoint = currentTarget.position;
        //    nextWaypoint.y = transform.position.y;
        //    float baseMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.BaseMoveSpeed);
        //    float adjustedMoveSpeed = baseMoveSpeed * chaseMoveSpeedModifier;
        //    entityInformation.SetAttribute(EntityAttributes.NextWaypoint, nextWaypoint);
        //    entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, adjustedMoveSpeed);
        //}
        //else if (isChasing)
        //{
        //    Vector3 nextWaypoint = GenerateChaseMovementPosition();
        //    float baseMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.BaseMoveSpeed);
        //    float adjustedMoveSpeed = baseMoveSpeed * chaseMoveSpeedModifier;
        //    entityInformation.SetAttribute(EntityAttributes.NextWaypoint, nextWaypoint);
        //    entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, adjustedMoveSpeed);
        //}
        //else
        //{
        //    Vector3 nextWaypoint = GenerateCombatMovementPosition();
        //    float baseMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.BaseMoveSpeed);
        //    float adjustedMoveSpeed = baseMoveSpeed * combatMoveSpeedModifier;
        //    entityInformation.SetAttribute(EntityAttributes.NextWaypoint, nextWaypoint);
        //    entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, adjustedMoveSpeed);
        //}

        Vector3 nextWaypoint = new Vector3(UnityEngine.Random.RandomRange(-10f, 10f), 0f, UnityEngine.Random.RandomRange(-10f, 10f));
        entityInformation.SetAttribute(EntityAttributes.NextWaypoint, nextWaypoint);

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }

    Vector3 GenerateCombatMovementPosition()
    {
        return transform.position;
    }

    Vector3 GenerateChaseMovementPosition()
    {
        currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        Vector3 toTarget = currentTarget.position - transform.position;
        Vector3 clampedFromTarget = Vector3.ClampMagnitude((transform.position - currentTarget.position), attackRange * 2 / 3);

        Vector3 chaseWaypoint = toTarget + clampedFromTarget;
        chaseWaypoint.y = transform.position.y;

        return chaseWaypoint;
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
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, StopFunctioning);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopFunctioning);

            entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
            entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        }
    }
    #endregion
}