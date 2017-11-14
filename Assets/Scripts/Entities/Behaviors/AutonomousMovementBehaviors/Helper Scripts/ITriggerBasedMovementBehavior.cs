using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerBasedMovementBehavior
{
    void RegisterCollider(Collider collider);
    void DeregisterCollider(Collider collider);
}
