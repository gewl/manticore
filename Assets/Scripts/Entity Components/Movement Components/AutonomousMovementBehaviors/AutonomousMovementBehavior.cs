using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutonomousMovementBehavior {

    protected abstract int priority { get; set; }
    public int Priority { get { return priority; } }

    public abstract Vector3 CalculateForce(AutonomousMovementComponent movementComponent);
}
