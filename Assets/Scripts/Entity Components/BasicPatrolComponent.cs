using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPatrolComponent : EntityComponent {

    [SerializeField]
    Transform[] patrolPoints;

    int patrolPointer = 0;
    int pauseTimer = 0;

    public override void Initialize()
    {
        if (patrolPoints.Length <= 1)
        {
            throw new System.Exception("Not enough patrol points in BasicPatrolComponent.");
        }
        base.entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);
        base.entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
        base.entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);

        SetNewWaypoint();
    }

    public override void Cleanup()
    {
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
    }

    #region EntityEvent handlers

    void OnUpdate()
    {
        if (pauseTimer > 0)
        {
            pauseTimer--;
            if (pauseTimer == 0)
            {
                base.entityEmitter.EmitEvent(EntityEvents.Move);
            }
        }
    }

    void OnWaypointReached()
    {
        base.entityEmitter.EmitEvent(EntityEvents.Stop);
        pauseTimer = 30;

        SetNewWaypoint();
    }

    void OnAggro()
    {
        this.enabled = false;
    }

    #endregion

    void SetNewWaypoint()
    {
        Transform nextWaypoint = patrolPoints[patrolPointer];
        base.entityData.SetSoftAttribute(SoftEntityAttributes.NextWaypoint, nextWaypoint.transform.position);
        patrolPointer++;
        if (patrolPointer >= patrolPoints.Length)
        {
            patrolPointer = 0;
        }
    }
}
