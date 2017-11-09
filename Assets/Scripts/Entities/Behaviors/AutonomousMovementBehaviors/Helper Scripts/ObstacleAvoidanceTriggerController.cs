using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceTriggerController : MonoBehaviour {

    ObstacleAvoidance obstacleAvoidanceBehavior;

    public void RegisterControllingBehavior(ObstacleAvoidance behavior)
    {
        obstacleAvoidanceBehavior = behavior;
    }

    void OnTriggerEnter(Collider other)
    {
        obstacleAvoidanceBehavior.RegisterCollider(other);
    }

    void OnTriggerExit(Collider other)
    {
        obstacleAvoidanceBehavior.DeregisterCollider(other);
    }
}
