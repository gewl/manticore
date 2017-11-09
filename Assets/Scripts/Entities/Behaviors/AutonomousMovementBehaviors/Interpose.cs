using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpose : AutonomousMovementBehavior {

    Transform primaryTarget;
    Transform secondaryTarget;
    Rigidbody primaryTargetRigidbody;
    Rigidbody secondaryTargetRigidbody;

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
        if (movementComponent.primaryTarget == null || movementComponent.secondaryTarget == null)
        {
            return Vector3.zero;
        }

        if (primaryTarget != movementComponent.primaryTarget || secondaryTarget != movementComponent.secondaryTarget)
        {
            primaryTarget = movementComponent.primaryTarget;
            secondaryTarget = movementComponent.secondaryTarget;

            primaryTargetRigidbody = primaryTarget.GetComponent<Rigidbody>();
            secondaryTargetRigidbody = secondaryTarget.GetComponent<Rigidbody>();
        }

        Vector3 targetMidpoint = CalculateInterposePoint(movementComponent, primaryTarget, secondaryTarget, primaryTargetRigidbody.velocity, secondaryTargetRigidbody.velocity);
        Vector3 toTargetMidpoint = targetMidpoint - movementComponent.transform.position;

        return arrive.ArriveToPosition(toTargetMidpoint, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

    public Vector3 CalculateInterposePoint(AutonomousMovementComponent movementComponent, Transform primaryTarget, Transform secondaryTarget, Vector3 primaryTargetVelocity, Vector3 secondaryTargetVelocity)
    {
        Vector3 midpointBetweenTargets = (primaryTarget.position + secondaryTarget.position) / 2f;

        float timeToReachMidpoint = (midpointBetweenTargets - movementComponent.transform.position).magnitude / movementComponent.maxSpeed;

        Vector3 updatedPrimaryTargetPosition = primaryTarget.position + (primaryTargetVelocity * timeToReachMidpoint);
        Vector3 updatedSecondaryTargetPosition = secondaryTarget.position + (secondaryTargetVelocity * timeToReachMidpoint);

        Vector3 updatedMidpoint = (updatedPrimaryTargetPosition + updatedSecondaryTargetPosition) / 2f;

        return updatedMidpoint;
    }
}
