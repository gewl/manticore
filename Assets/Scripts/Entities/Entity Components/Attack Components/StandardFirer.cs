using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFirer : EntityComponent {

    [SerializeField]
    Transform bulletsParent;
    [SerializeField]
    Transform projectile;
    [SerializeField]
    Transform firer;

    IStandardFirerData _standardFirerData;
    IStandardFirerData standardFirerData
    {
        get
        {
            if (_standardFirerData == null)
            {
                _standardFirerData = entityInformation.Data as IStandardFirerData;
            }

            return _standardFirerData;
        }
    }

    float projectileStrength { get { return standardFirerData.ProjectileStrength; } }
    float bulletSpeed { get { return standardFirerData.BulletSpeed; } }
    float aimNoiseInDegrees { get { return standardFirerData.AimNoiseInDegrees; } }

    List<Vector3> cachedTargetVelocities;

    int maximumVelocitiesToCache = 5;
    int currentVelocityCacheIndex = 0;

    Transform target;
    Rigidbody targetRigidbody;

    // Experimenting with enemies beginning shooting inaccurately (large amount
    // of noise added to shots), then accuracy increases over period of time until
    // noise floor reached. Should allow players to get the sense of a room at the outset
    // of an encounter, then gradually be more and more pressured as time passes.
    [SerializeField]
    float timeToWarmUp = 5f;
    [SerializeField]
    float maximumWarmUpNoiseModifier = 3f;
    float timeAggroed;
    float timeWarmedUp;

    enum FireType { Direct, Lead }
    [SerializeField]
    FireType currentFireType = FireType.Lead;

    protected override void OnEnable()
    {
        base.OnEnable();
        cachedTargetVelocities = new List<Vector3>();
        for (int i = 0; i < maximumVelocitiesToCache; i++)
        {
            cachedTargetVelocities.Add(Vector3.zero);
        }
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.TargetUpdated, OnTargetUpdated);
    }

    #region EntityEvent handlers

    void OnPrimaryFire()
    {
        Transform currentTarget = (Transform)entityInformation.GetAttribute(EntityAttributes.CurrentTarget);
        FireProjectile(currentTarget);
    }

    void OnAggro()
    {
        timeAggroed = Time.time;
        timeWarmedUp = timeAggroed + timeToWarmUp;
    }

    void OnTargetUpdated()
    {
        cachedTargetVelocities.Clear();
        currentVelocityCacheIndex = 0;
    }

    #endregion

    void FireProjectile(Transform currentTarget)
    {
        if (currentTarget != target)
        {
            target = currentTarget;
            targetRigidbody = target.GetComponent<Rigidbody>();
        }
        Vector3 relativePos = currentTarget.position - firer.transform.position;

        if (currentFireType == FireType.Lead)
        {
            float timeToImpact = relativePos.sqrMagnitude / (bulletSpeed * bulletSpeed);
            Vector3 currentTargetVelocity = targetRigidbody.velocity;

            cachedTargetVelocities.Add(currentTargetVelocity);
            currentVelocityCacheIndex++;
            if (currentVelocityCacheIndex >= maximumVelocitiesToCache)
            {
                currentVelocityCacheIndex = 0;
            }

            Vector3 cumulativeVelocity = Vector3.zero;
            for (int i = 0; i < cachedTargetVelocities.Count; i++)
            {
                cumulativeVelocity += cachedTargetVelocities[i];
            }

            Vector3 averageVelocity = cumulativeVelocity / maximumVelocitiesToCache;
            averageVelocity *= timeToImpact;
            relativePos += averageVelocity;
        }

        float baseNoiseAdjustment = Random.Range(-aimNoiseInDegrees, aimNoiseInDegrees);
        relativePos = Vector3.RotateTowards(relativePos, transform.right, Mathf.Deg2Rad * baseNoiseAdjustment, 1);

        float currentTime = Time.time;
        if (currentTime < timeWarmedUp)
        {
            //ApplyWarmUpNoise(ref relativePos, currentTime);
        }
        relativePos.y = 0f;

        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        Transform createdBullet = Object.Instantiate(projectile, firer.position, rotation);
        BasicBullet bulletController = createdBullet.GetComponent<BasicBullet>();
        bulletController.strength = projectileStrength;
        bulletController.targetPosition = relativePos;
        bulletController.firer = transform;
        bulletController.target = currentTarget;
        bulletController.speed = bulletSpeed;

        createdBullet.transform.parent = bulletsParent.transform;
    }

    void ApplyWarmUpNoise(ref Vector3 aimPosition, float currentTime)
    {
        float timeToWarmedUp = timeWarmedUp - currentTime;
        float percentageOfNoiseToApply = timeToWarmedUp / timeToWarmUp;

        float noiseToApply = (percentageOfNoiseToApply * maximumWarmUpNoiseModifier) * aimNoiseInDegrees;

        Vector3.RotateTowards(aimPosition, transform.right, Mathf.Deg2Rad * noiseToApply, 1);
    }

}
