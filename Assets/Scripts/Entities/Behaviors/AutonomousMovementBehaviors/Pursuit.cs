using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : AutonomousMovementBehavior {

    Transform target;
    Rigidbody targetRigidbody;

    Seek _seek;
    Seek seek
    {
        get
        {
            if (_seek == null)
            {
                _seek = new Seek();
            }

            return _seek;
        }
    }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (movementComponent.PursuitTarget != target)
        {
            target = movementComponent.PursuitTarget;
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
        //if (Vector3.Dot(toTarget, movementComponent.transform.forward) > 0 && Vector3.Dot(movementComponent.transform.forward, target.forward) < -0.95f)
        //{
        //    return seek.CalculateForce(movementComponent);
        //}

        float movementProjectionTime = toTarget.magnitude / (movementComponent.maxSpeed + targetVelocity.magnitude);

        Vector3 updatedTargetPosition = targetPosition + (targetVelocity * movementProjectionTime);

        return seek.SeekToPosition(agentPosition, updatedTargetPosition, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

}
