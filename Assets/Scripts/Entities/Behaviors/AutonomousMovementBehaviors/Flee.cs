using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AutonomousMovementBehavior {

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        return FleeFromPosition(movementComponent.transform.position, movementComponent.FleeTarget.position, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

    public Vector3 FleeFromPosition(Vector3 agentPosition, Vector3 targetPosition, float maximumSpeed, Vector3 currentVelocity)
    {
        Vector3 desiredVelocity = (agentPosition - targetPosition).normalized * maximumSpeed;

        return desiredVelocity - currentVelocity;
    }
}
