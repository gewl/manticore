using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : AutonomousMovementBehavior {

    float minimumTriggerLength = 5f;

    BoxCollider obstacleAvoidanceTrigger;

    public override Vector3 CalculateForce(AutonomousMovementComponent movementComponent)
    {
        if (obstacleAvoidanceTrigger == null)
        {
            obstacleAvoidanceTrigger = InitializeObstacleAvoidanceTrigger(movementComponent.gameObject);
        }

        UpdateTriggerSize(movementComponent.EntityRigidbody.velocity.magnitude, movementComponent.maxSpeed);

        return Vector3.zero;
    }

    void UpdateTriggerSize(float currentSpeed, float maxSpeed)
    {
        float updatedZSize = minimumTriggerLength + (currentSpeed / maxSpeed) * minimumTriggerLength;
        Vector3 currentSize = obstacleAvoidanceTrigger.size;
        obstacleAvoidanceTrigger.size = new Vector3(currentSize.x, currentSize.y, updatedZSize);
        obstacleAvoidanceTrigger.center = new Vector3(0f, 0f, (updatedZSize / 2f) - 1f);
    }

    BoxCollider InitializeObstacleAvoidanceTrigger(GameObject parentObject)
    {
        GameObject avoidanceTriggerContainer = new GameObject("Obstacle Avoidance Trigger");
        avoidanceTriggerContainer.transform.SetParent(parentObject.transform);
        avoidanceTriggerContainer.transform.localPosition = Vector3.zero;
        BoxCollider instantiatedTrigger = avoidanceTriggerContainer.AddComponent<BoxCollider>();
        instantiatedTrigger.isTrigger = true;
        instantiatedTrigger.size = parentObject.GetComponent<MeshRenderer>().bounds.size;

        return instantiatedTrigger;
    }
}
