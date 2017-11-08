using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : AutonomousMovementBehavior {

    protected override int priority { get { return 2; } set { } }

    Vector3 wanderTarget;

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        float wanderDistance = movementComponent.WanderDistance;
        float wanderRadius = movementComponent.WanderRadius;
        float wanderJitter = movementComponent.WanderJitter;

        return generateNewWanderTarget(movementComponent.transform.forward, wanderDistance, wanderRadius, wanderJitter);
    }

    Vector3 generateNewWanderTarget(Vector3 forward, float wanderDistance, float wanderRadius, float wanderJitter)
    {
        if (wanderTarget == null)
        {
            wanderTarget = Vector3.forward;
        }

        wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0f, Random.Range(-1f, 1f) * wanderJitter);
        wanderTarget = wanderTarget.normalized * wanderRadius;

        Vector3 displacedWanderTarget = wanderTarget + (forward * wanderDistance);

        return displacedWanderTarget;
    }

}
