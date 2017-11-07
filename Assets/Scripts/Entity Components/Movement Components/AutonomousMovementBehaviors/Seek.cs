using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : AutonomousMovementBehavior {

    protected override int priority { get { return 1; } set { } }

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        return Vector3.zero;
    }
}
