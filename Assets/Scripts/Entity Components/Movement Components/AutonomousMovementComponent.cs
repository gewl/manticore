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

    // A number of the implementations of behaviors from the Buckland book involve some eyeball-y values for
    // individual features—things like deceleration in Arrive, distance to hide behind things in Hide, etc.
    // Rather than set up a bunch of situational cases to allow for this component tweaking individual behaviors at run-time,
    // this is an attempt to establish a single (manipulable) value to account for all these specific traits, on the assumption that
    // differing values correspond roughly to responsive/dextrous/graceful/precise vs. clumsy/sloppy/error-prone.
    [Range(0, 5)]
    public int Clumsiness = 3;

    [SerializeField]
    float maximumSteeringForce = 50f;
    public float maxSpeed = 50f;

    public Transform currentTarget;
    Rigidbody entityRigidbody;
    public Vector3 CurrentVelocity { get { return entityRigidbody.velocity; } }

    bool isOnARamp;
    int groundedCount = 0;

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

    protected override void Start()
    {
        base.Start();
        entityData.SetAttribute(EntityAttributes.CurrentTarget, currentTarget);
        entityEmitter.EmitEvent(EntityEvents.TargetUpdated);
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
        if (!isOnARamp && groundedCount == 0)
        {
            entityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }

        AccumulateForce();
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

        entityRigidbody.AddForce(accumulatedForce);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            groundedCount++;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            groundedCount--;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = false;
        }
    }
}
