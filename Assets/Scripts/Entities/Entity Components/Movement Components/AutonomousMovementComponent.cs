using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class AutonomousMovementComponent : EntityComponent {

    bool isConnected = false;

    public enum MovementBehaviorTypes
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
        OffsetPursuit,
        Separation,
        Alignment,
        Flocking, 
        Cohesion
    }

    [Title("Behaviors dictating entity movement", "Sorted in decreasing order of priority")]
    [SerializeField]
    private List<MovementBehaviorTypes> movementBehaviors;
    private List<AutonomousMovementBehavior> activeMovementBehaviors;

    [SerializeField]
    float maximumSteeringForce = 50f;
    public float MaxSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }

    // A number of the implementations of behaviors from the Buckland book involve some eyeball-y values for
    // individual features—things like deceleration in Arrive, distance to hide behind things in Hide, etc.
    // Rather than set up a bunch of situational cases to allow for this component tweaking individual behaviors at run-time,
    // this is an attempt to establish a single (manipulable) value to account for all these specific traits, on the assumption that
    // differing values correspond roughly to responsive/dextrous/graceful/precise vs. clumsy/sloppy/error-prone.
    [Range(0, 5)]
    public int Clumsiness = 3;

    [Header("Wander Configuration")]
    [SerializeField, HideInInspector]
    float wanderDistance = 2f;
    public float WanderDistance { get { return wanderDistance; } }
    [SerializeField, HideInInspector]
    float wanderRadius = 1f;
    public float WanderRadius { get { return wanderRadius; } }
    [SerializeField, HideInInspector]
    float wanderJitter = 0.5f;
    public float WanderJitter { get { return wanderJitter; } }

    #region Serialized fields for individual behaviors
    [Header("Path Following Configuration")]
    [OdinSerialize, HideInInspector]
    public List<Transform> PathNodes;
    [HideInInspector]
    public float pauseBetweenPathLegs = 0.5f;

    [Header("Interpose Configuration")]
    [HideInInspector]
    public Transform PrimaryInterposeTarget;
    [HideInInspector]
    public Transform SecondaryInterposeTarget;

    [Header("Seek Configuration")]
    [HideInInspector]
    public Transform SeekTarget;
    [HideInInspector]
    public Vector3 SeekLocation;

    [Header("Pursuit Configuration")]
    [HideInInspector]
    public Transform PursuitTarget;

    [Header("Offset Pursuit Configuration")]
    [HideInInspector]
    public Transform OffsetPursuitTarget;
    [OdinSerialize, HideInInspector]
    public Vector3 PursuitOffset;

    [Header("Hide Configuration")]
    [HideInInspector]
    public Transform HideTarget;

    [Header("Flee Configuration")]
    [HideInInspector]
    public Transform FleeTarget;

    [Header("Evade Configuration")]
    [HideInInspector]
    public Transform EvadeTarget;

    [Header("Arrive Configuration")]
    [HideInInspector]
    public Transform ArriveTarget;
    [HideInInspector]
    public Vector3 ArriveLocation;

    [Header("Separation Configuration")]
    [HideInInspector]
    public float SeparationRadius;

    [Header("Alignment Configuration")]
    [HideInInspector]
    public float AlignmentRadius;

    [Header("Cohesion Configuration")]
    [HideInInspector]
    public float CohesionRadius;
    #endregion

    Rigidbody entityRigidbody;
    public Rigidbody EntityRigidbody { get { return entityRigidbody; } }
    public Vector3 CurrentVelocity { get { return entityRigidbody.velocity; } }

    bool isOnARamp;
    int groundedCount = 0;

    #region Lifecycle/component subscription functions
    protected override void Awake()
    {
        base.Awake();
        entityRigidbody = GetComponent<Rigidbody>();

        GenerateNewActiveMovementBehaviorList();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GenerateNewActiveMovementBehaviorList();
        }
#endif
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Move, Connect);
        entityEmitter.SubscribeToEvent(EntityEvents.Stop, Disconnect);
        if (!isConnected)
        {
            Connect();
        }
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, Connect);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, Disconnect);

        if (isConnected)
        {
            Disconnect();
        }
    }

    void Connect()
    {
        if (!isConnected)
        {
            entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
            isConnected = true;
        }
    }

    void Disconnect()
    {
        if (isConnected)
        {
            entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
            isConnected = false;
            entityRigidbody.velocity = Vector3.zero;
        }
    }

    void OnFixedUpdate()
    {
        if (!isOnARamp && groundedCount == 0)
        {
            entityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }
        //if (ArriveTarget != null)
        //{
        //    Vector3 toTarget = ArriveTarget.position - transform.position;
        //    if (toTarget.sqrMagnitude < 0.1f)
        //    {
        //        entityRigidbody.velocity = Vector3.zero;
        //        return;
        //    }
        //}

        AccumulateForce();
    }
    #endregion

    #region List manipulation
    AutonomousMovementBehavior GetMovementBehaviorClass(MovementBehaviorTypes behaviorType)
    {
        string ns = typeof(MovementBehaviorTypes).Namespace;

        string typeName = ns + "." + behaviorType.ToString();

        return (AutonomousMovementBehavior)Activator.CreateInstance(Type.GetType(typeName));
    }

    void ClearMovementBehaviorList()
    {
        activeMovementBehaviors.Clear();
    }

    void GenerateNewActiveMovementBehaviorList()
    {
        activeMovementBehaviors = new List<AutonomousMovementBehavior>();

        for (int i = 0; i < movementBehaviors.Count; i++)
        {
            AutonomousMovementBehavior behaviorToAdd = GetMovementBehaviorClass(movementBehaviors[i]);

            activeMovementBehaviors.Add(behaviorToAdd);
        }
    }
    #endregion

    void AccumulateForce()
    {
        Vector3 accumulatedForce = Vector3.zero;
        for (int i = 0; i < activeMovementBehaviors.Count; i++)
        {
            AutonomousMovementBehavior behavior = activeMovementBehaviors[i];

            Vector3 resultantForce = behavior.CalculateForce(this);

            float magnitudeOfResultantForce = resultantForce.magnitude;
            float remainingForce = maximumSteeringForce - accumulatedForce.magnitude;

            if (magnitudeOfResultantForce <= remainingForce)
            {
                accumulatedForce += resultantForce;
            }
            else
            {
                resultantForce = Vector3.ClampMagnitude(resultantForce, remainingForce);
                accumulatedForce += resultantForce;
                break;
            }
        }

        accumulatedForce.y = 0f;

        float accumulatedForceSqrMag = accumulatedForce.sqrMagnitude;

        entityRigidbody.AddForce(accumulatedForce, ForceMode.VelocityChange);
        entityRigidbody.velocity = Vector3.ClampMagnitude(entityRigidbody.velocity, MaxSpeed);
    }

    #region Event Handlers
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
    #endregion  
}
