using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : AutonomousMovementBehavior {

    Transform target;
    Rigidbody targetRigidbody;

    Vector3[] _cachedPositions;
    Vector3[] CachedPositions
    {
        get
        {
            if (_cachedPositions == null)
            {
                _cachedPositions = new Vector3[positionsToCache];
            }

            return _cachedPositions;
        }
    }
    int positionsToCache = 40;
    int nextUpdateIndex;

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
        if (Vector3.Dot(toTarget, movementComponent.transform.forward) > 0 && Vector3.Dot(movementComponent.transform.forward, target.forward) < -0.95f)
        {
            return seek.SeekToPosition(agentPosition, targetPosition, movementComponent.MaxSpeed, agentVelocity);
        }

        float movementProjectionTime = toTarget.magnitude / (movementComponent.MaxSpeed + targetVelocity.magnitude);

        Vector3 updatedTargetPosition = SmoothTargetPosition(targetPosition + (targetVelocity * movementProjectionTime));

        return seek.SeekToPosition(agentPosition, updatedTargetPosition, movementComponent.MaxSpeed, movementComponent.CurrentVelocity);
    }

    Vector3 SmoothTargetPosition(Vector3 newTargetPosition)
    {
        CachedPositions[nextUpdateIndex++] = newTargetPosition;

        if (nextUpdateIndex == positionsToCache)
        {
            nextUpdateIndex = 0;
        }

        Vector3 averagedPosition = Vector3.zero;
        for (int i = 0; i < positionsToCache; i++)
        {
            averagedPosition += CachedPositions[i];
        }

        return averagedPosition / positionsToCache;
    }

}
