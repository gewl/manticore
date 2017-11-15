using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : AutonomousMovementBehavior, ITriggerBasedMovementBehavior
{
    const string entityTriggerName = "Entity Trigger";
    const int entityTriggerLayer = 22;

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
            entityTrigger = InitializeEntityTrigger(movementComponent.gameObject, movementComponent.SeparationRadius);
        }

        Vector3 agentPosition = movementComponent.transform.position;
        Vector3 steeringForce = Vector3.zero;

        HashSet<Transform>.Enumerator colliderEnumerator = TaggedNeighbors.GetEnumerator();

        while (colliderEnumerator.MoveNext())
        {
            Transform neighbor = colliderEnumerator.Current;
            Vector3 toAgent = agentPosition - neighbor.position;

            Vector3 responseForce = (toAgent.normalized / toAgent.magnitude);
            steeringForce += responseForce;
        }

        return steeringForce;
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
