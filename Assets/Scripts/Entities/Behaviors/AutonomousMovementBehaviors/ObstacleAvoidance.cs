using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : AutonomousMovementBehavior, ITriggerBasedMovementBehavior {

    float minimumTriggerLength = 6f;

    BoxCollider obstacleAvoidanceTrigger;
    const string obstacleAvoidanceTriggerName = "Obstacle Avoidance Trigger";
    const float brakingWeight = 0.3f;

    #region Collider management
    HashSet<Collider> _taggedColliders;
    HashSet<Collider> TaggedColliders
    {
        get
        {
            if (_taggedColliders == null)
            {
                _taggedColliders = new HashSet<Collider>();
            }
            return _taggedColliders;
        }
    }

    public void RegisterCollider(Collider collider)
    {
        TaggedColliders.Add(collider);
    }

    public void DeregisterCollider(Collider collider)
    {
        TaggedColliders.Remove(collider);
    }
    #endregion

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (obstacleAvoidanceTrigger == null)
        {
            obstacleAvoidanceTrigger = InitializeObstacleAvoidanceTrigger(movementComponent.gameObject);
        }

        UpdateTriggerSize(movementComponent.EntityRigidbody.velocity.magnitude, movementComponent.MaxSpeed, movementComponent.transform);

        if (TaggedColliders.Count == 0)
        {
            return Vector3.zero;
        }

        // In local coordinate system, viewed from above, agent's "forward" is along Z-axis ('vertically'), agent's "right"
        // is along X-axis ("horizontally"). PGAIBE assumes that agent's "forward" is along x-axis ('horizontally') so there's
        // some inconsistency.
        Vector3 steeringForce = Vector3.zero;
        float avoidanceTriggerLength = obstacleAvoidanceTrigger.size.z;
        Collider closestCollider = null;
        float closestZDistance = float.MaxValue;

        HashSet<Collider>.Enumerator colliderEnumerator = TaggedColliders.GetEnumerator();

        while (colliderEnumerator.MoveNext())
        {
            Collider collider = colliderEnumerator.Current;
            Transform colliderTransform = collider.transform;

            float zDistanceToCollider = movementComponent.transform.InverseTransformPoint(colliderTransform.position).z;

            if (zDistanceToCollider < closestZDistance)
            {
                closestZDistance = zDistanceToCollider;
                closestCollider = collider;
            }
        }

        float steeringWeight = 1.5f + (avoidanceTriggerLength - closestZDistance) / avoidanceTriggerLength;
        Vector3 relativePositionOfCollider = movementComponent.transform.InverseTransformPoint(closestCollider.transform.position);

        float effectiveObstacleRadius = closestCollider.bounds.extents.x;

        steeringForce.x = (effectiveObstacleRadius - relativePositionOfCollider.x) * steeringWeight;
        steeringForce.z = (effectiveObstacleRadius - closestZDistance) * brakingWeight;

        return movementComponent.transform.TransformVector(steeringForce);
    }

    #region Trigger management
    void UpdateTriggerSize(float currentSpeed, float maxSpeed, Transform agentTransform)
    {
        float updatedZSize = minimumTriggerLength + (currentSpeed / maxSpeed) * minimumTriggerLength;
        Vector3 currentSize = obstacleAvoidanceTrigger.size;
        obstacleAvoidanceTrigger.size = new Vector3(currentSize.x, currentSize.y, updatedZSize);
        obstacleAvoidanceTrigger.center = new Vector3(0f, 0f, (updatedZSize / 2f) - (agentTransform.localScale.z / 2f));
    }

    BoxCollider InitializeObstacleAvoidanceTrigger(GameObject parentObject)
    {
        GameObject avoidanceTriggerContainer = new GameObject(obstacleAvoidanceTriggerName)
        {
            layer = LayerMask.NameToLayer("ObstacleAvoidanceTrigger")
        };
        MovementBehaviorTriggerController triggerController = avoidanceTriggerContainer.AddComponent<MovementBehaviorTriggerController>();
        triggerController.RegisterControllingBehavior(this);

        avoidanceTriggerContainer.transform.SetParent(parentObject.transform);
        avoidanceTriggerContainer.transform.localPosition = Vector3.zero;
        avoidanceTriggerContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);

        BoxCollider instantiatedTrigger = avoidanceTriggerContainer.AddComponent<BoxCollider>();
        instantiatedTrigger.isTrigger = true;
        instantiatedTrigger.size = new Vector3(parentObject.transform.localScale.x * 1.7f, parentObject.transform.localScale.y * 0.8f, parentObject.transform.localScale.z);

        return instantiatedTrigger;
    }
    #endregion
}