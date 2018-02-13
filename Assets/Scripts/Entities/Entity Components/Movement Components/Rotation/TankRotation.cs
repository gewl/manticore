using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRotation : EntityComponent {

    Rigidbody entityRigidbody;

    [Header("Body Rotation")]
    [SerializeField]
    bool isSmoothing = true;
    [SerializeField]
    int velocitiesToCache = 5;

    int nextUpdateSlot;
    Vector3[] cachedVelocities;

    RangedEntityData _rangedEntityData;
    RangedEntityData rangedEntityData
    {
        get
        {
            if (_rangedEntityData == null)
            {
                _rangedEntityData = entityInformation.Data as RangedEntityData;
            }

            return _rangedEntityData;
        }
    }

    float RotationStrength { get { return rangedEntityData.RotationStrength; } }

    [Header("Head Rotation")]
    [SerializeField]
    Transform headBone;
    [SerializeField]
    Transform currentTarget;

    float lastCachedHeadAngle;

    protected override void Awake()
    {
        base.Awake();
        entityRigidbody = GetComponent<Rigidbody>();

        cachedVelocities = new Vector3[velocitiesToCache];

        for (int i = 0; i < velocitiesToCache; i++)
        {
            cachedVelocities[i] = Vector3.zero;
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, StopFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, StopFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.FreezeRotation, StopFunctioning);

        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, UpdateTarget);

        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, BeginFunctioning);
        entityEmitter.SubscribeToEvent(EntityEvents.ResumeRotation, BeginFunctioning);


        BeginFunctioning();
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, StopFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FreezeRotation, StopFunctioning);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, UpdateTarget);

        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, BeginFunctioning);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ResumeRotation, BeginFunctioning);

        StopFunctioning();
    }

    void UpdateTarget()
    {
        Transform newTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);

        currentTarget = newTarget;
    }

    void BeginFunctioning()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.LateUpdate, OnLateUpdate);
    }

    void StopFunctioning()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.LateUpdate, OnLateUpdate);
    }

    void OnFixedUpdate()
    {
        Vector3 currentVelocity = entityRigidbody.velocity;
        if (currentVelocity.sqrMagnitude >= 0.1f)
        {
            if (isSmoothing)
            {
                transform.rotation = Quaternion.LookRotation(SmoothVelocity(currentVelocity));
            }
            else
            {
                Quaternion nextRotation = Quaternion.LookRotation(currentVelocity);
                transform.rotation = nextRotation;
            }
        }
    }

    Vector3 SmoothVelocity(Vector3 mostRecentVelocity)
    {
        cachedVelocities[nextUpdateSlot++] = mostRecentVelocity;

        if (nextUpdateSlot == velocitiesToCache)
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

    void OnLateUpdate()
    {
        float angleToTarget = 0f;
        if (currentTarget != null)
        {
            Vector3 currentForward = transform.forward;
            angleToTarget = AngleSigned(currentForward, currentTarget.position - transform.position, Vector3.up);

            angleToTarget += 180f;
        }

        Vector3 currentRotation = transform.rotation.eulerAngles;

        float str = Mathf.Min(RotationStrength * Time.deltaTime, 1f);

        // Without this, the head's rotation always takes "the long way around" when the
        // target passes behind the entity containing this component.
        if (Mathf.Abs(lastCachedHeadAngle - angleToTarget) > 180f)
        {
            if (lastCachedHeadAngle > 180f)
            {
                lastCachedHeadAngle -= 360f;
            } 
            else
            {
                angleToTarget -= 360f;
            }
        }

        float updatedZAngle = Mathf.Lerp(lastCachedHeadAngle, angleToTarget, str);

        lastCachedHeadAngle = updatedZAngle;

        // TODO: Hardcoded values here to deal w/ weirdness across Unity/Blender axis systems;
        // could see this causing problems w/ different coordinate paradigms in the future.
        headBone.rotation = Quaternion.Euler(currentRotation.x - 90f, currentRotation.y - 90f, updatedZAngle + 90f);
    }

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
}
