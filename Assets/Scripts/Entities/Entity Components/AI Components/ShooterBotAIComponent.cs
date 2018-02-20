using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterBotAIComponent : EntityComponent {

    AutonomousMovementComponent movementComponent;
    NavMeshAgent navMeshAgent;

    bool isAggroed = false;
    bool isChasing = false;

    public List<Transform> patrolNodes;
    public List<Vector3> patrolPositions;
    int currentPatrolPositionIndex = 0;
    bool reachedNewPatrolPoint = false;

    [SerializeField]
    float minPatrolPause = 0.1f;
    [SerializeField]
    float maxPatrolPause = 0.5f;

    [SerializeField]
    float minCombatPause = 0.1f;
    [SerializeField]
    float maxCombatPause = 0.5f;

    RangedEntityData _entityData;
    RangedEntityData EntityData
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

    float FireCooldown { get { return EntityData.AttackCooldown; } }
    float ArcOfFire { get { return EntityData.ArcOfFire; } }
    float AttackRange { get { return EntityData.AttackRange; } }

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
    [SerializeField]
    Transform headBone;
    Transform firer;
    Renderer firerRenderer;
    Color firerOriginalSkin;

    protected override void Awake()
    {
        base.Awake();

        movementComponent = GetComponent<AutonomousMovementComponent>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        patrolPositions = new List<Vector3>
        {
            transform.position
        };

        for (int i = 0; i < patrolNodes.Count; i++)
        {
            patrolPositions.Add(patrolNodes[i].position);
        }

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
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, Reconnect);

        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.Deaggro, OnDeaggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, Disconnect);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, Disconnect);
        if (isAggroed)
        {
            OnAggro();
        }
        timeElapsedSinceLastFire = 0f;
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, Reconnect);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, Disconnect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, Disconnect);
    }

    #region Reused subscription bundles
    void OnAggro()
    {
        isAggroed = true;
        entityEmitter.EmitEvent(EntityEvents.Stop);
    }

    void OnDeaggro()
    {
        isAggroed = false;
    }

    void Reconnect()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    void Disconnect()
    {
        CancelInvoke();
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
    }
    #endregion

    #region EntityEvent handlers

    void OnUpdate()
    {
        navMeshAgent.speed = entityStats.GetMoveSpeed();
        if (isAggroed)
        {
            AggroedUpdate();
        }
        else
        {
            UnaggroedUpdate();
        }
    }

    void UnaggroedUpdate()
    {
        float sqrDistanceToNextPatrolPosition = (patrolPositions[currentPatrolPositionIndex] - transform.position).sqrMagnitude;
        if (sqrDistanceToNextPatrolPosition <= 0.5f && !reachedNewPatrolPoint)
        {
            entityEmitter.EmitEvent(EntityEvents.Stop);

            float patrolPause = UnityEngine.Random.Range(minPatrolPause, maxPatrolPause);
            reachedNewPatrolPoint = true;
            Invoke("UpdatePatrolPosition", patrolPause);
        }
    }

    void UpdatePatrolPosition()
    {
        currentPatrolPositionIndex++;

        if (currentPatrolPositionIndex >= patrolPositions.Count)
        {
            currentPatrolPositionIndex = 0;
        }

        navMeshAgent.SetDestination(patrolPositions[currentPatrolPositionIndex]);
        reachedNewPatrolPoint = false;

        entityEmitter.EmitEvent(EntityEvents.Move);
    }

    void AggroedUpdate()
    {
        if (timeElapsedSinceLastFire < FireCooldown)
        {
            timeElapsedSinceLastFire += Time.deltaTime;

            float percentageComplete = timeElapsedSinceLastFire / FireCooldown;
            firerRenderer.material.color = Color.Lerp(firerOriginalSkin, Color.red, gunHeatCurve.Evaluate(percentageComplete));
        }
        else
        {
            if (CanFirePrimary())
            {
                timeElapsedSinceLastFire = 0f;
                firerRenderer.material.color = firerOriginalSkin;
            }
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

    void OnWaypointReached()
    {
        Debug.Log("Wayponit reached");
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        isChasing = false;

        Invoke("GenerateAndSetWaypoint", UnityEngine.Random.Range(minimumMovementPause, maximumMovementPause));
    }

    #endregion

    #region discrete functions to offload event listeners

    bool CanFirePrimary()
    {
        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        Vector3 directionToTarget = currentTarget.position - transform.position;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        float firerFacingAngle = headBone.localRotation.eulerAngles.y - 180f;

        float angleBetweenFirerAndTarget = (angleToTarget - Mathf.Abs(firerFacingAngle));

        if (Mathf.Abs(angleBetweenFirerAndTarget) <= ArcOfFire && IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.PrimaryFire);
            timeElapsedSinceLastFire = FireCooldown;
            return true;
        }
        else
        {
            return false;
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
        Vector3 clampedFromTarget = Vector3.ClampMagnitude((transform.position - currentTarget.position), AttackRange * 2 / 3);

        Vector3 chaseWaypoint = toTarget + clampedFromTarget;
        chaseWaypoint.y = transform.position.y;

        return chaseWaypoint;
    }

    bool IsInRange(Transform target)
    {
        float squaredDistanceToTarget = (target.position - transform.position).sqrMagnitude;

        if (squaredDistanceToTarget >= AttackRange * AttackRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

}