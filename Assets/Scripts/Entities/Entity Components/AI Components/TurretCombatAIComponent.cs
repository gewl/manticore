using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.Serialization;

public class TurretCombatAIComponent : EntityComponent {

    bool isAggroed = false;
    LayerMask allButTerrainMask;
    Bounds entityBounds;

    [SerializeField]
    float basePercentageOfRangeFromTarget = 0.66f;
    [SerializeField]
    float percentageOfRangeMovementVariance = 0.25f;
    [SerializeField]
    float rotationAroundTargetMinimumVariance = 60f;
    [SerializeField]
    float rotationAroundTargetMaximumVariance = 120f;

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
        entityBounds = GetComponent<Collider>().bounds;

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