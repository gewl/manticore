using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHeadToTarget : EntityComponent {

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

    Quaternion lastCachedHeadAngle = Quaternion.identity;

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
        entityEmitter.SubscribeToEvent(EntityEvents.LateUpdate, CalculateHeadRotation);
    }

    void StopFunctioning()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.LateUpdate, CalculateHeadRotation);
    }
    void CalculateHeadRotation()
    {
        if (currentTarget == null)
        {
            return;
        }

        if (lastCachedHeadAngle == Quaternion.identity)
        {
            lastCachedHeadAngle = headBone.transform.rotation;
        }

        Vector3 toTarget = currentTarget.position - headBone.transform.position;
        Vector3 targetRotationEuler = Quaternion.LookRotation(toTarget).eulerAngles;
        targetRotationEuler.x = 0f;
        targetRotationEuler.z = 0f;

        float adjustedRotationStrength = Mathf.Min(RotationStrength * Time.deltaTime, 1f);

        Quaternion adjustedRotation = Quaternion.Lerp(lastCachedHeadAngle, Quaternion.Euler(targetRotationEuler), adjustedRotationStrength);
        headBone.transform.rotation = adjustedRotation;

        lastCachedHeadAngle = adjustedRotation;
    }
}
