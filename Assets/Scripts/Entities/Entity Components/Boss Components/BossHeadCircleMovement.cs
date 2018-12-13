using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossHeadCircleMovement : EntityComponent {

    [SerializeField]
    GameObject bossWaypointsParent;

    bool wasInterrupted = false;

    Transform[] waypoints;
    int nextWaypointPointer = 0;
    Animator animator;
    const string IS_MOVING = "isMoving";

    Vector3 patrolPointOffset;

    [SerializeField]
    float travelTime = 2f;
    [SerializeField]
    float lingerTime = 2f;

    List<EntityEmitter> bodyPartEntityEmitters;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        bodyPartEntityEmitters = GetComponent<GroupReferenceComponent>()
            .GetGroup()
            .Select(m => m.GetComponent<EntityEmitter>())
            .ToList();
    }

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

        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, Move);
        entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
        entityEmitter.SubscribeToEvent(EntityEvents.Move, Move);
        entityEmitter.EmitEvent(EntityEvents.WaypointReached);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, Move);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, Move);
    }

    void OnStop()
    {
        StopAllCoroutines();
        BroadcastToBodyParts(EntityEvents.Stop);
        wasInterrupted = true;
    }

    void Move()
    {
        StartCoroutine(WaitThenTravelToNextNode());
    }

    void BroadcastToBodyParts(string entityEvent)
    {
        if (entityEvent == EntityEvents.Move)
        {
            animator.SetBool(IS_MOVING, true);
        }
        else if (entityEvent == EntityEvents.Stop)
        {
            animator.SetBool(IS_MOVING, false);
        }

        foreach (EntityEmitter emitter in bodyPartEntityEmitters)
        {
            if (emitter == null)
            {
                continue;
            }

            emitter.EmitEvent(entityEvent);
        }
    }

    IEnumerator WaitThenTravelToNextNode()
    {
        yield return new WaitForSeconds(lingerTime);

        if (!wasInterrupted)
        {
            nextWaypointPointer += 2;
            if (nextWaypointPointer >= waypoints.Length)
            {
                nextWaypointPointer = 0;
            }
        }
        else
        {
            wasInterrupted = false;
        }

        BroadcastToBodyParts(EntityEvents.Move);
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

        BroadcastToBodyParts(EntityEvents.Stop);
        entityEmitter.EmitEvent(EntityEvents.WaypointReached);
    }

}
