using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.Serialization;

public class ShooterBotAIComponent : EntityComponent {

    NavMeshAgent navMeshAgent;

    bool isAggroed = false;
    LayerMask allButTerrainMask;
    Bounds entityBounds;

    [NonSerialized, OdinSerialize]
    public List<Transform> patrolNodes;
    List<Vector3> patrolPositions;
    int currentPatrolPositionIndex = 0;
    bool reachedDestination = false;

    [SerializeField]
    float minPatrolPause = 0.1f;
    [SerializeField]
    float maxPatrolPause = 0.5f;
    [SerializeField]
    float unaggroedMovementModifier = 0.5f;

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

        allButTerrainMask = 1 << LayerMask.NameToLayer("Terrain");
        navMeshAgent = GetComponent<NavMeshAgent>();
        entityBounds = GetComponent<Collider>().bounds;

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
        CancelInvoke();
        isAggroed = true;
        reachedDestination = false;
        navMeshAgent.destination = transform.position;
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
        navMeshAgent.isStopped = true;
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
    }

    #endregion

    #region EntityEvent handlers

    void OnUpdate()
    {
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
        if (sqrDistanceToNextPatrolPosition <= 0.5f && !reachedDestination)
        {
            entityEmitter.EmitEvent(EntityEvents.Stop);

            float patrolPause = UnityEngine.Random.Range(minPatrolPause, maxPatrolPause);
            reachedDestination = true;
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
        reachedDestination = false;

        navMeshAgent.speed = EntityData.BaseMoveSpeed * unaggroedMovementModifier;
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

        Vector3 currentDestination = navMeshAgent.destination;

        float sqrDistanceToCurrentDestination = (currentDestination - transform.position).sqrMagnitude;

        if (sqrDistanceToCurrentDestination < 2f && !reachedDestination)
        {
            entityEmitter.EmitEvent(EntityEvents.Stop);

            float movementPause = UnityEngine.Random.Range(minCombatPause, maxCombatPause);
            reachedDestination = true;
            Invoke("GenerateCombatMovementPosition", movementPause);
        }
    }

    #endregion

    #region discrete functions to offload event listeners

    bool CanFirePrimary()
    {
        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        Vector3 directionToTarget = currentTarget.position - transform.position;
        float angleToTarget = Vector3.Angle(headBone.forward, directionToTarget);

        if (Mathf.Abs(angleToTarget) <= ArcOfFire && IsInRange(currentTarget))
        {
            entityEmitter.EmitEvent(EntityEvents.PrimaryFire);
            return true;
        }
        else
        {
            return false;
        }
    }

    void GenerateCombatMovementPosition()
    {
        currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);

        Vector3 targetToSelf = transform.position - currentTarget.position;

        float minimumDistanceFromTarget = AttackRange * 0.2f;
        float maximumDistanceFromTarget = AttackRange * 0.8f;

        NavMeshHit sampleHit;
        Vector3 vectorFromTarget = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.RandomRange(minimumDistanceFromTarget, maximumDistanceFromTarget);
        Vector3 samplePosition = currentTarget.position + vectorFromTarget;
        NavMesh.SamplePosition(samplePosition, out sampleHit, maximumDistanceFromTarget, 1);

        Vector3 targetPosition = sampleHit.position;
        navMeshAgent.SetDestination(samplePosition);
        navMeshAgent.speed = EntityData.BaseMoveSpeed;
        entityEmitter.EmitEvent(EntityEvents.Move);

        reachedDestination = false;
    }

    Vector3 AdjustPositionForUninterruptedLoS(Vector3 currentTargetPosition, Vector3 desiredDestination)
    {
        RaycastHit hitInfo;
        if (!Physics.Linecast(currentTargetPosition, desiredDestination, out hitInfo, allButTerrainMask, QueryTriggerInteraction.Ignore))
        {
            return desiredDestination;
        }
        else
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3 targetToHit = hitPoint - currentTargetPosition;
            float distanceFromTargetAdjustedForBounds = hitInfo.distance - (entityBounds.extents.x * 2f);

            return currentTargetPosition + (targetToHit.normalized * distanceFromTargetAdjustedForBounds);
        }
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