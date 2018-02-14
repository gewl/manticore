using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : AutonomousMovementBehavior {

    Transform target;
    Rigidbody targetRigidbody;

    Flee _flee;
    Flee flee
    {
        get
        {
            if (_flee == null)
            {
                _flee = new Flee();
            }

            return _flee;
        }
    }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (movementComponent.EvadeTarget != target)
        {
            target = movementComponent.EvadeTarget;
            targetRigidbody = target.GetComponent<Rigidbody>();

#if UNITY_EDITOR
            if (targetRigidbody == null)
            {
                Debug.Log("Target does not have a rigidbody for Pursuit behavior in " + movementComponent.gameObject.name);
            }
#endif
        }

        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 targetPosition = target.position;

        Vector3 agentVelocity = movementComponent.EntityRigidbody.velocity;
        Vector3 targetVelocity = targetRigidbody.velocity;

        Vector3 toTarget = (targetPosition - agentPosition);
        if (Vector3.Dot(toTarget, movementComponent.transform.forward) > 0 && Vector3.Dot(movementComponent.transform.forward, target.forward) < -0.95f)
        {
            return flee.FleeFromPosition(agentPosition, targetPosition, movementComponent.maxSpeed, agentVelocity);
        }

        float movementProjectionTime = toTarget.magnitude / (movementComponent.maxSpeed + targetVelocity.magnitude);

        Vector3 updatedTargetPosition = targetPosition + (targetVelocity * movementProjectionTime);

        return flee.FleeFromPosition(agentPosition, updatedTargetPosition, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

}
