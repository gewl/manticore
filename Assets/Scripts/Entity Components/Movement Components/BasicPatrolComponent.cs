using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BasicPatrolComponent updates the entity's waypoints by iterating linearly through a serialized array of WAYPOINTS, pausing briefly after reaching a given waypoint.
/// </summary>
public class BasicPatrolComponent : EntityComponent {

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

        // This check should rarely be relevant, just want to avoid a condition where
        // the entity aggros very quickly & this component somehow sneaks a waypoint
        // at the right time to override the component responsible for handling movement
        // in combat.
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
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
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

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }
}
