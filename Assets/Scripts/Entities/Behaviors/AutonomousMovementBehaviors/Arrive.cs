using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : AutonomousMovementBehavior {

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 targetPosition = movementComponent.ArriveLocation;

        if (movementComponent.ArriveTarget != null)
        {
            targetPosition = movementComponent.ArriveTarget.position;
        }

        Vector3 toTarget = targetPosition - agentPosition;
        int deceleration = movementComponent.Clumsiness;

        return ArriveToPosition(toTarget, movementComponent.MaxSpeed, movementComponent.CurrentVelocity, deceleration);
    }

    public Vector3 ArriveToPosition(Vector3 toTarget, float maxSpeed, Vector3 currentVelocity, int deceleration = 1)
    {
        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget <= 0.2f)
        {
            return Vector3.zero;
        }

        float speedToReachTarget = distanceToTarget / (deceleration * 0.2f);

        speedToReachTarget = Mathf.Min(speedToReachTarget, maxSpeed);

        Vector3 desiredVelocity = toTarget * speedToReachTarget / distanceToTarget;

        return desiredVelocity - currentVelocity;
    }

}
