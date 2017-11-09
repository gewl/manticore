using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : AutonomousMovementBehavior {

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 targetPosition = movementComponent.currentTarget.position;

        Vector3 toTarget = targetPosition - agentPosition;
        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget <= 0.1f)
        {
            return Vector3.zero;
        }

        int deceleration = movementComponent.Clumsiness;

        float speedToReachTarget = distanceToTarget / (deceleration * 0.2f);

        speedToReachTarget = Mathf.Min(speedToReachTarget, movementComponent.maxSpeed);

        Vector3 desiredVelocity = toTarget * speedToReachTarget / distanceToTarget;

        return desiredVelocity - movementComponent.CurrentVelocity;
    }

}
