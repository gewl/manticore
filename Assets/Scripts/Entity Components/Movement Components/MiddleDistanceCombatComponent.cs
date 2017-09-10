using System;
using UnityEngine;

public class MiddleDistanceCombatComponent : EntityComponent {

    [SerializeField]
    float minimumPause;
    [SerializeField]
    float maximumPause;
    [SerializeField]
    float minimumDistanceToMove = 15;
    [SerializeField]
    float maximumDistanceToMove = 25;

    Vector3 nextWaypoint;

    protected override void Subscribe()
    {
        if (minimumDistanceToMove > maximumDistanceToMove || minimumDistanceToMove < 0 || maximumDistanceToMove <= 0)
        {
            throw new Exception("Invalid range for entity's 'distance to move' values.");
        }

        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);
        GenerateAndMoveToWaypoint();
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
    }

    #region EntityEvent handlers

    void OnWaypointReached()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);

        Invoke("GenerateAndMoveToWaypoint", UnityEngine.Random.Range(minimumPause, maximumPause));
    }

    #endregion

    void GenerateAndMoveToWaypoint()
    {
        Vector3 nextWaypoint = GenerateCombatMovementPosition();
        entityData.SetSoftAttribute(SoftEntityAttributes.NextWaypoint, nextWaypoint);

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }

    public Vector3 GenerateCombatMovementPosition()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        Vector3 currentPositionDifference = entityData.EntityTransform.position - currentTarget.position;

        Vector3 tempWaypoint = currentPositionDifference.normalized;
        tempWaypoint.x = tempWaypoint.x + UnityEngine.Random.Range(-.5f, .5f);
        tempWaypoint.z = tempWaypoint.z + UnityEngine.Random.Range(-.5f, .5f);

        tempWaypoint.x = currentTarget.position.x + (tempWaypoint.x * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.z = currentTarget.position.z + (tempWaypoint.z * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.y = transform.position.y;

        return tempWaypoint;
    }
}
