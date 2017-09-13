using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicPatrolComponent updates the entity's waypoints by iterating linearly through a serialized array of WAYPOINTS, pausing briefly after reaching a given waypoint.
/// </summary>
public class BasicPatrolComponent : EntityComponent {

    [SerializeField]
    float patrolMoveSpeedModifier = 1f;
    [SerializeField]
    Transform[] patrolPoints;
    [SerializeField]
    float pauseTimer;

    int patrolPointer = 0;

    // This initialization assumes that the entity is beginning in a deaggroed state.
    // It seems a little rigid, but it should be very unusual that an entity spawns aggroed;
    // especially since "spawns aggroed" functionality could be managed by creating new component,
    // increasing range on BasicAggroComponent to cover point that player enters the level, etc.
    protected override void Subscribe()
    {
        if (patrolPoints.Length <= 1)
        {
            throw new System.Exception("Not enough patrol points in BasicPatrolComponent.");
        }
        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Deaggro, OnDeaggro);

		GenerateAndMoveToWaypoint();
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
	}

    #region EntityEvent handlers

    void OnAggro()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
        CancelInvoke();
    }

    void OnDeaggro()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);

        GenerateAndMoveToWaypoint();
	}

    void OnWaypointReached()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);

        Invoke("GenerateAndMoveToWaypoint", pauseTimer);
    }

    #endregion

    void SetNewWaypoint()
    {
        Transform nextWaypoint = patrolPoints[patrolPointer];
        entityData.SetSoftAttribute(SoftEntityAttributes.NextWaypoint, nextWaypoint.transform.position);
        patrolPointer++;
        if (patrolPointer >= patrolPoints.Length)
        {
            patrolPointer = 0;
        }
    }

    // To be used with Invoke in OnWaypointReached for pause effect before moving again.
    void GenerateAndMoveToWaypoint()
    {
        SetNewWaypoint();
        float baseMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.BaseMoveSpeed);
        float adjustedMoveSpeed = baseMoveSpeed * patrolMoveSpeedModifier;
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, adjustedMoveSpeed);

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }
}
