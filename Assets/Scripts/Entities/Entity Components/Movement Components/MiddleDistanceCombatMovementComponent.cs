using System;
using UnityEngine;

// Deprecated: Wrapped into CubeCombatAIComponent.
public class MiddleDistanceCombatMovementComponent : EntityComponent {

    [SerializeField]
    float combatMoveSpeed;
    [SerializeField]
    float minimumPause;
    [SerializeField]
    float maximumPause;
    [SerializeField]
    float minimumDistanceToMove = 15;
    [SerializeField]
    float maximumDistanceToMove = 25;

    Vector3 nextWaypoint;

    // This is assuming that the entity will not spawn aggroed.
    protected override void Subscribe()
    {
        if (minimumDistanceToMove > maximumDistanceToMove || minimumDistanceToMove < 0 || maximumDistanceToMove <= 0)
        {
            throw new Exception("Invalid range for entity's 'distance to move' values.");
        }

        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Deaggro, OnDeaggro);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
    }

    #region EntityEvent handlers

    void OnWaypointReached()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);

        Invoke("GenerateAndMoveToWaypoint", UnityEngine.Random.Range(minimumPause, maximumPause));
    }

    void OnAggro()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.WaypointReached, OnWaypointReached);

        GenerateAndMoveToWaypoint();
    }

    void OnDeaggro()
    {
        entityEmitter.EmitEvent(EntityEvents.ClearWaypoint);
        CancelInvoke();

        entityEmitter.UnsubscribeFromEvent(EntityEvents.WaypointReached, OnWaypointReached);
    }

    #endregion

    void GenerateAndMoveToWaypoint()
    {
        Vector3 nextWaypoint = GenerateCombatMovementPosition();
        entityInformation.SetAttribute(EntityAttributes.NextWaypoint, nextWaypoint);
        entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, combatMoveSpeed);

        entityEmitter.EmitEvent(EntityEvents.SetWaypoint);
    }

    public Vector3 GenerateCombatMovementPosition()
    {
        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        Vector3 currentPositionDifference = entityInformation.EntityTransform.position - currentTarget.position;

        Vector3 tempWaypoint = currentPositionDifference.normalized;
        tempWaypoint.x = tempWaypoint.x + UnityEngine.Random.Range(-.5f, .5f);
        tempWaypoint.z = tempWaypoint.z + UnityEngine.Random.Range(-.5f, .5f);

        tempWaypoint.x = currentTarget.position.x + (tempWaypoint.x * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.z = currentTarget.position.z + (tempWaypoint.z * UnityEngine.Random.Range(minimumDistanceToMove, maximumDistanceToMove));
        tempWaypoint.y = transform.position.y;

        return tempWaypoint;
    }
}
