using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : AutonomousMovementBehavior {

    protected override int priority { get { return 1; } set { } }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        return SeekToPosition(movementComponent.transform.position, movementComponent.currentTarget.position, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

    public Vector3 SeekToPosition(Vector3 agentPosition, Vector3 targetPosition, float maximumSpeed, Vector3 currentVelocity)
    {
        Vector3 desiredVelocity = (targetPosition - agentPosition).normalized * maximumSpeed;
        Debug.DrawLine(agentPosition, targetPosition, Color.blue, 0.1f);

        return desiredVelocity - currentVelocity;
    }
}
