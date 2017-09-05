using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPatrolComponent : EntityComponent {

    [SerializeField]
    Transform[] patrolPoints;

    int patrolPointer = 0;
    int pauseTimer = 0;

    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        if (patrolPoints.Length <= 1)
        {
            throw new System.Exception("Not enough patrol points in BasicPatrolComponent.");
        }
        base.entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);
        base.entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);

        SetNewWaypoint();
    }

    public override void Cleanup()
    {
        base.entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
    }

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
