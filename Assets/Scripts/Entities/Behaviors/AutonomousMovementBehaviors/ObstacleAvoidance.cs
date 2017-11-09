using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : AutonomousMovementBehavior {

    float minimumTriggerLength = 6f;

    BoxCollider obstacleAvoidanceTrigger;
    const string obstacleAvoidanceTriggerName = "Obstacle Avoidance Trigger";
    const int obstacleAvoidanceTriggerLayer = 21;
    const float brakingWeight = 1f;

    #region Collider management
    List<Collider> _taggedColliders;
    List<Collider> taggedColliders
    {
        get
        {
            if (_taggedColliders == null)
            {
                _taggedColliders = new List<Collider>();
            }
            return _taggedColliders;
        }
    }

    public void RegisterCollider(Collider newCollider)
    {
        if (!taggedColliders.Contains(newCollider))
        {
            taggedColliders.Add(newCollider);
        }
    }

    public void DeregisterCollider(Collider newCollider)
    {
        taggedColliders.Remove(newCollider);
    }
    #endregion

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (obstacleAvoidanceTrigger == null)
        {
            obstacleAvoidanceTrigger = InitializeObstacleAvoidanceTrigger(movementComponent.gameObject);
        }

        UpdateTriggerSize(movementComponent.EntityRigidbody.velocity.magnitude, movementComponent.maxSpeed, movementComponent.transform);

        if (taggedColliders.Count == 0)
        {
            return Vector3.zero;
        }

        // In local coordinate system, viewed from above, agent's "forward" is along Z-axis ('vertically'), agent's "right"
        // is along X-axis ("horizontally"). PGAIBE assumes that agent's "forward" is along x-axis ('horizontally') so there's
        // some inconsistency.
        Vector3 steeringForce = Vector3.zero;
        float avoidanceTriggerLength = obstacleAvoidanceTrigger.size.z;
        Collider closestCollider = taggedColliders[0];
        float closestZDistance = movementComponent.transform.InverseTransformPoint(closestCollider.transform.position).z;

        for (int i = 1; i < taggedColliders.Count; i++)
        {
            Collider collider = taggedColliders[i];
            Transform colliderTransform = collider.transform;

            float zDistanceToCollider = movementComponent.transform.InverseTransformPoint(colliderTransform.position).z;

            if (zDistanceToCollider < closestZDistance)
            {
                closestZDistance = zDistanceToCollider;
                closestCollider = collider;
            }
        }

        float steeringWeight = 1f + (avoidanceTriggerLength - closestZDistance) / avoidanceTriggerLength;
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
            layer = obstacleAvoidanceTriggerLayer
        };
        ObstacleAvoidanceTriggerController triggerController = avoidanceTriggerContainer.AddComponent<ObstacleAvoidanceTriggerController>();
        triggerController.RegisterControllingBehavior(this);

        avoidanceTriggerContainer.transform.SetParent(parentObject.transform);
        avoidanceTriggerContainer.transform.localPosition = Vector3.zero;
        avoidanceTriggerContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);

        BoxCollider instantiatedTrigger = avoidanceTriggerContainer.AddComponent<BoxCollider>();
        instantiatedTrigger.isTrigger = true;
        instantiatedTrigger.size = new Vector3(parentObject.transform.localScale.x * 1.2f, parentObject.transform.localScale.y * 0.8f, parentObject.transform.localScale.z);

        return instantiatedTrigger;
    }
    #endregion
}