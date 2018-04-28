using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityWatcherMovementComponent : EntityComponent {

    bool isMoving = false;
    float velocityMagnitudeThreshold = 0.08f;

    Rigidbody entityRigidbody;

    protected override void Subscribe()
    {
        entityRigidbody = GetComponent<Rigidbody>();

        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.Move, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, OnMove);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
    }

    void OnFixedUpdate()
    {
        if (!isMoving && entityRigidbody.velocity.sqrMagnitude >= velocityMagnitudeThreshold)
        {
            entityEmitter.EmitEvent(EntityEvents.Move);
        }
        else if (isMoving && entityRigidbody.velocity.sqrMagnitude < velocityMagnitudeThreshold)
        {
            entityEmitter.EmitEvent(EntityEvents.Stop);
        }
    }

    void OnMove()
    {
        isMoving = true;
    }

    void OnStop()
    {
        isMoving = false;
    }
}
