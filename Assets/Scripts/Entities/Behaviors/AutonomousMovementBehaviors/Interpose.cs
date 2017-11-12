using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpose : AutonomousMovementBehavior {

    Transform primaryInterposeTarget;
    Transform secondaryInterposeTarget;
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
        if (movementComponent.PrimaryInterposeTarget == null || movementComponent.SecondaryInterposeTarget == null)
        {
            return Vector3.zero;
        }

        if (primaryInterposeTarget != movementComponent.PrimaryInterposeTarget || secondaryInterposeTarget != movementComponent.SecondaryInterposeTarget)
        {
            primaryInterposeTarget = movementComponent.PrimaryInterposeTarget;
            secondaryInterposeTarget = movementComponent.SecondaryInterposeTarget;

            primaryTargetRigidbody = primaryInterposeTarget.GetComponent<Rigidbody>();
            secondaryTargetRigidbody = secondaryInterposeTarget.GetComponent<Rigidbody>();
        }

        Vector3 targetMidpoint = CalculateInterposePoint(movementComponent, primaryInterposeTarget, secondaryInterposeTarget, primaryTargetRigidbody.velocity, secondaryTargetRigidbody.velocity);
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
