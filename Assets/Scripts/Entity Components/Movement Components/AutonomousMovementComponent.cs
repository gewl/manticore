using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousMovementComponent : EntityComponent {

    enum MovementBehaviorTypes
    {
        Seek,
        Flee,
        Arrive,
        Pursuit,
        Evade,
        Wander,
        ObstacleAvoidance,
        WallAvoidance,
        Interpose,
        Hide,
        PathFollowing,
        OffsetPursuit
    }

    [SerializeField]
    private List<MovementBehaviorTypes> movementBehaviors;
    private SortedList<AutonomousMovementBehavior, int> activeMovementBehaviors;

    [SerializeField]
    float maximumSteeringForce = 10f;

    public Transform currentTarget;
    Rigidbody entityRigidbody;

    protected override void Awake()
    {
        base.Awake();
        entityRigidbody = GetComponent<Rigidbody>();

        activeMovementBehaviors = new SortedList<AutonomousMovementBehavior, int>(new BehaviorComparer());

        for (int i = 0; i < movementBehaviors.Count; i++)
        {
            AutonomousMovementBehavior behaviorToAdd = GetMovementBehaviorClass(movementBehaviors[i]);

            activeMovementBehaviors.Add(behaviorToAdd, behaviorToAdd.Priority);
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, AccumulateForce);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, AccumulateForce);
    }

    AutonomousMovementBehavior GetMovementBehaviorClass(MovementBehaviorTypes behaviorType)
    {
        string ns = typeof(MovementBehaviorTypes).Namespace;

        string typeName = ns + "." + behaviorType.ToString();

        return (AutonomousMovementBehavior)Activator.CreateInstance(Type.GetType(typeName));
    }

    void AccumulateForce()
    {
        float magnitudeRemaining = maximumSteeringForce;
        Vector3 accumulatedForce = Vector3.zero;
        foreach (KeyValuePair<AutonomousMovementBehavior, int> activeBehavior in activeMovementBehaviors)
        {
            AutonomousMovementBehavior behavior = activeBehavior.Key;

            Vector3 resultantForce = behavior.CalculateForce(this);

            float magnitudeOfResultantForce = resultantForce.magnitude;

            if (magnitudeOfResultantForce < magnitudeRemaining)
            {
                magnitudeRemaining -= magnitudeOfResultantForce;
                accumulatedForce += resultantForce;
            }
            else
            {
                resultantForce = resultantForce.normalized * magnitudeRemaining;
                accumulatedForce += resultantForce;
                break;
            }
        }
    }

    #region Behavior collection management

    internal class BehaviorComparer : IComparer<AutonomousMovementBehavior>
    {
        public int Compare (AutonomousMovementBehavior x, AutonomousMovementBehavior y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }

    #endregion
}
