using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursuit : AutonomousMovementBehavior {

    Transform target;
    Rigidbody targetRigidbody;

    Arrive _arrive;
    Arrive arrive
    {
        get
        {
            if (_arrive == null)
            {
                _arrive = new Arrive();
            }

            return _arrive;
        }
    }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (target != movementComponent.OffsetPursuitTarget)
        {
            target = movementComponent.OffsetPursuitTarget;
            targetRigidbody = target.GetComponent<Rigidbody>();
        }
        Vector3 worldOffsetPosition = target.position + movementComponent.PursuitOffset;

        Vector3 toOffset = worldOffsetPosition - movementComponent.transform.position;
        float timeToOffset = toOffset.magnitude / (movementComponent.MaxSpeed + targetRigidbody.velocity.magnitude);
        Vector3 updatedTarget = worldOffsetPosition + (targetRigidbody.velocity * timeToOffset);

        return arrive.ArriveToPosition(updatedTarget - movementComponent.transform.position, movementComponent.MaxSpeed, movementComponent.CurrentVelocity, 1);
    }
}
