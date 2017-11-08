using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : AutonomousMovementBehavior {

    protected override int priority { get { return 2; } set { } }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 targetPosition = movementComponent.currentTarget.position;

        Vector3 fromTarget = agentPosition - targetPosition;
        fromTarget = fromTarget.normalized * movementComponent.maxSpeed;
        return fromTarget - movementComponent.CurrentVelocity;
    }
}
