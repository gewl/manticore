using UnityEngine;

public class MovementBehaviorTriggerController : MonoBehaviour {

    ITriggerBasedMovementBehavior movementBehavior;

    public void RegisterControllingBehavior(ITriggerBasedMovementBehavior behavior)
    {
        movementBehavior = behavior;
    }

    void OnTriggerEnter(Collider other)
    {
        movementBehavior.RegisterCollider(other);
    }

    void OnTriggerExit(Collider other)
    {
        movementBehavior.DeregisterCollider(other);
    }
}
