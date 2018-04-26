using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossHeadCircleMovement : EntityComponent {

    [SerializeField]
    GameObject bossWaypointsParent;

    Transform[] waypoints;
    int nextWaypointPointer = 0;

    Vector3 patrolPointOffset;

    [SerializeField]
    float travelTime = 2f;
    [SerializeField]
    float lingerTime = 2f;

    protected override void Subscribe()
    {
        waypoints = new Transform[bossWaypointsParent.transform.childCount];

        int waypointPopulationIndex = 0;
        foreach (Transform waypoint in bossWaypointsParent.transform)
        {
            waypoints[waypointPopulationIndex] = waypoint;
            waypointPopulationIndex++;
        }
        patrolPointOffset = transform.position - waypoints[0].position;

        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);
        entityEmitter.EmitEvent(EntityEvents.WaypointReached);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
    }

    void OnWaypointReached()
    {
        StartCoroutine(WaitThenTravelToNextNode());
    }

    IEnumerator WaitThenTravelToNextNode()
    {
        yield return new WaitForSeconds(lingerTime);

        nextWaypointPointer += 2;
        if (nextWaypointPointer >= waypoints.Length)
        {
            nextWaypointPointer = 0;
        }

        Vector3 nextNodePosition = waypoints[nextWaypointPointer].position + patrolPointOffset;
        Quaternion nextNodeRotation = waypoints[nextWaypointPointer].rotation;

        float timeElapsed = 0f;
        Vector3 startingPosition = transform.position;
        Quaternion startingRotation = transform.rotation;

        while (timeElapsed < travelTime)
        {
            float percentageComplete = timeElapsed / travelTime;
            float curvedCompletion = GameManager.BelovedSwingCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(startingPosition, nextNodePosition, curvedCompletion);
            transform.rotation = Quaternion.Lerp(startingRotation, nextNodeRotation, curvedCompletion);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        entityEmitter.EmitEvent(EntityEvents.WaypointReached);
    }
}
