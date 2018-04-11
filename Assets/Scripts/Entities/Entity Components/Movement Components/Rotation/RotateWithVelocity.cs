using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RotateWithVelocity : EntityComponent {

    NavMeshAgent navMeshAgent;
    bool isSmoothing = true;
    [SerializeField]
    int velocitiesToCache = 5;

    int nextUpdateSlot;
    Vector3[] cachedVelocities;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = GetComponent<NavMeshAgent>();
        cachedVelocities = new Vector3[velocitiesToCache];

        for (int i = 0; i < cachedVelocities.Length; i++)
        {
            cachedVelocities[i] = Vector3.zero;
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnFixedUpdate()
    {
        if (navMeshAgent.velocity.sqrMagnitude >= 0.1f)
        {
            if (isSmoothing)
            {
                transform.rotation = Quaternion.LookRotation(SmoothVelocity(navMeshAgent.velocity));
            }
            else
            {
                Quaternion nextRotation = Quaternion.LookRotation(navMeshAgent.velocity);
                transform.rotation = nextRotation;
            }
        }
    }

    Vector3 SmoothVelocity(Vector3 mostRecentVelocity)
    {
        cachedVelocities[nextUpdateSlot++] = mostRecentVelocity;

        if (nextUpdateSlot == cachedVelocities.Length)
        {
            nextUpdateSlot = 0;
        }

        Vector3 averagedVelocity = Vector3.zero;

        for (int i = 0; i < velocitiesToCache; i++)
        {
            averagedVelocity += cachedVelocities[i];
        }

        averagedVelocity.y = 0f;
        return averagedVelocity / velocitiesToCache;
    }
}
