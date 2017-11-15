using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : AutonomousMovementBehavior, ITriggerBasedMovementBehavior {

    const string entityTriggerName = "Entity Trigger";
    const int entityTriggerLayer = 22;

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

    SphereCollider entityTrigger;
    
    #region Tagged neighbors management
    HashSet<Transform> _taggedNeighbors;
    HashSet<Transform> TaggedNeighbors
    {
        get
        {
            if (_taggedNeighbors == null)
            {
                _taggedNeighbors = new HashSet<Transform>();
            }
            return _taggedNeighbors;
        }
    }

    public void RegisterCollider(Collider collider)
    {
        TaggedNeighbors.Add(collider.transform);
    }

    public void DeregisterCollider(Collider collider)
    {
        TaggedNeighbors.Remove(collider.transform);
    }
    #endregion

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (entityTrigger == null)
        {
            entityTrigger = InitializeEntityTrigger(movementComponent.gameObject, movementComponent.CohesionRadius);
        }
        if (TaggedNeighbors.Count == 0)
        {
            return Vector3.zero;
        }

        HashSet<Transform>.Enumerator neighborEnumerator = TaggedNeighbors.GetEnumerator();
        Vector3 centerOfMass = Vector3.zero;

        while (neighborEnumerator.MoveNext())
        {
            Transform neighbor = neighborEnumerator.Current;

            centerOfMass += neighbor.position;
        }

        centerOfMass /= TaggedNeighbors.Count;

        return seek.SeekToPosition(movementComponent.transform.position, centerOfMass, movementComponent.maxSpeed, movementComponent.CurrentVelocity);
    }

    SphereCollider InitializeEntityTrigger(GameObject parentObject, float triggerRadius)
    {
        GameObject entityTriggerContainer = new GameObject(entityTriggerName)
        {
            layer = LayerMask.NameToLayer("EntityTrigger")
        };
        MovementBehaviorTriggerController triggerController = entityTriggerContainer.AddComponent<MovementBehaviorTriggerController>();
        triggerController.RegisterControllingBehavior(this);

        entityTriggerContainer.transform.SetParent(parentObject.transform);
        entityTriggerContainer.transform.localPosition = Vector3.zero;

        SphereCollider instantiatedTrigger = entityTriggerContainer.AddComponent<SphereCollider>();
        instantiatedTrigger.isTrigger = true;
        instantiatedTrigger.radius = triggerRadius;

        return instantiatedTrigger;
    }
}
