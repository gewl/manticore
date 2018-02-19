using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFirer : EntityComponent {

    [SerializeField]
    Transform bulletsParent;
    Transform firer;
    [SerializeField]
    Transform spawnPoint;

    RangedEntityData _standardFirerData;
    RangedEntityData StandardFirerData
    {
        get
        {
            if (_standardFirerData == null)
            {
                _standardFirerData = entityInformation.Data as RangedEntityData;
            }

            return _standardFirerData;
        }
    }

    float ProjectileStrength { get { return StandardFirerData.BaseDamage; } }
    float BulletSpeed { get { return StandardFirerData.BulletSpeed; } }
    float AimNoiseInDegrees { get { return StandardFirerData.AimNoiseInDegrees; } }
    Transform Projectile { get { return StandardFirerData.Projectile; } }

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
    float maximumWarmUpNoiseModifier = 10f;
    float timeAggroed;
    float timeWarmedUp;

    protected override void Awake()
    {
        base.Awake();
        firer = transform.FindChildByRecursive("Firer");

        if (spawnPoint == null)
        {
            spawnPoint = firer.transform;
        }
        spawnPoint.position = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z);
    }

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
        Vector3 targetPosition = currentTarget.position;

        // Lead bullet logic
        float timeToImpact = targetPosition.sqrMagnitude / (BulletSpeed * BulletSpeed);
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
        targetPosition += averageVelocity;

        float baseNoiseAdjustment = Random.Range(-AimNoiseInDegrees, AimNoiseInDegrees);
        targetPosition = RotatePointAroundPivot(targetPosition, transform.position, baseNoiseAdjustment);

        float currentTime = Time.time;
        if (currentTime < timeWarmedUp)
        {
            targetPosition = ApplyWarmUpNoiseToPoint(targetPosition, currentTime);
        }
        targetPosition.y = 0f;

        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        Transform createdBullet = Instantiate(Projectile, spawnPoint.position, rotation);
        BasicBullet bulletController = createdBullet.GetComponent<BasicBullet>();
        bulletController.strength = ProjectileStrength;
        bulletController.targetPosition = targetPosition;
        bulletController.firer = transform;
        bulletController.target = currentTarget;
        bulletController.speed = BulletSpeed;

        createdBullet.transform.parent = bulletsParent.transform;
    }

    Vector3 ApplyWarmUpNoiseToPoint(Vector3 aimPoint, float currentTime)
    {
        float timeToWarmedUp = timeWarmedUp - currentTime;
        float percentageOfNoiseToApply = timeToWarmedUp / timeToWarmUp;

        float degreesOfNoiseToApply = (percentageOfNoiseToApply * maximumWarmUpNoiseModifier) * AimNoiseInDegrees;

        float coinFlipForSign = Random.value;

        if (coinFlipForSign > 0.5f)
        {
            degreesOfNoiseToApply *= -1;
        }

        return RotatePointAroundPivot(aimPoint, transform.position, degreesOfNoiseToApply);
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float degreesToRotate)
    {
        Quaternion rotationAngles = Quaternion.Euler(0.0f, degreesToRotate, 0.0f);

        Vector3 direction = point - pivot;
        direction = rotationAngles * direction;
        return direction + pivot;
    }
}
