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

    public override void Initialize()
    {
        if (patrolPoints.Length <= 1)
        {
            throw new System.Exception("Not enough patrol points in BasicPatrolComponent.");
        }
        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);

        GenerateAndMoveToWaypoint();
    }

    public override void Cleanup()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
    }

    #region EntityEvent handlers

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
