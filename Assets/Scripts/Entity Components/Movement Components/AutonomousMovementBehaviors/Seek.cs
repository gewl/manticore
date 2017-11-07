using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : AutonomousMovementBehavior {

    protected override int priority { get { return 1; } set { } }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 targetPosition = movementComponent.currentTarget.position;

        Vector3 toTarget = targetPosition - agentPosition;
        toTarget = toTarget.normalized * movementComponent.maxSpeed;
        Debug.DrawLine(agentPosition, targetPosition, Color.red, 0.1f);
        return toTarget;
    }
}
