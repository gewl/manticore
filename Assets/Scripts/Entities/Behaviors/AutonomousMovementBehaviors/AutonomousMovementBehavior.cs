using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutonomousMovementBehavior {

    public abstract Vector3 CalculateForce(AutonomousMovementComponent movementComponent);
}
