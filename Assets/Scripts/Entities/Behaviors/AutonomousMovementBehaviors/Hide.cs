using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : AutonomousMovementBehavior {

    Vector3 hidingSpot;
    bool hasHidingSpot = false;
    Transform agent;
    Renderer agentRenderer;
    Rigidbody agentRigidbody;

    Vector3 targetPosition;

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
        if (agent != movementComponent.transform)
        {
            agent = movementComponent.transform;
            agentRenderer = agent.GetComponent<Renderer>();
            agentRigidbody = agent.GetComponent<Rigidbody>();
        }

        bool isAtHidingSpot = (hidingSpot != Vector3.zero && (movementComponent.transform.position - hidingSpot).sqrMagnitude < 0.1f);

        if (isAtHidingSpot)
        {
            if ((movementComponent.HideTarget.position - targetPosition).sqrMagnitude < 1f)
            {
                return Vector3.zero;
            }
            else
            {
                hasHidingSpot = false;
            }
        }

        if (!hasHidingSpot)
        {
            hidingSpot = GenerateHidingSpot(movementComponent.transform, movementComponent.HideTarget);
            hasHidingSpot = true;
            targetPosition = movementComponent.HideTarget.position;
        }

        return arrive.ArriveToPosition(hidingSpot - agent.position, movementComponent.MaxSpeed, agentRigidbody.velocity, 1);
    }

    Vector3 GenerateHidingSpot(Transform agent, Transform target)
    {
        Transform obstacleToHideBehind = GameManager.GetHidingSpot(agent, target);
        Vector3 directionFromTargetToObstacle = (obstacleToHideBehind.position - target.position).normalized;
        float hidingPositionBufferDistance = agentRenderer.bounds.size.z * 2f;

        return obstacleToHideBehind.position + (directionFromTargetToObstacle * hidingPositionBufferDistance);
    }
}
