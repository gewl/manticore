using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificTargetFirer : EntityComponent {

    [SerializeField]
    Transform target;

    Transform firer;
    Transform _spawnPoint;
    Transform SpawnPoint
    {
        get
        {
            if (_spawnPoint == null)
            {
                _spawnPoint = transform.FindChildByRecursive("BulletSpawn");
            }

            return _spawnPoint;
        }
    }

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

    float ProjectileStrength { get { return StandardFirerData.BaseDamage * entityStats.GetDamageDealtModifier(); } }
    float BulletSpeed { get { return StandardFirerData.BulletSpeed; } }
    float AimNoiseInDegrees { get { return StandardFirerData.AimNoiseInDegrees; } }
    Transform Projectile { get { return StandardFirerData.Projectile; } }

    List<Vector3> cachedTargetVelocities;

    int maximumVelocitiesToCache = 5;
    int currentVelocityCacheIndex = 0;

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

        if (SpawnPoint == null)
        {
            _spawnPoint = firer.transform;
        }
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
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
    }

    #region EntityEvent handlers

    public void FireProjectile()
    {
        Transform currentTarget = target;
        FireProjectile(currentTarget);
    }

    void OnAggro()
    {
        timeAggroed = Time.time;
        timeWarmedUp = timeAggroed + timeToWarmUp;
    }

    #endregion

    void FireProjectile(Transform currentTarget)
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        Transform createdBullet = Instantiate(Projectile, SpawnPoint.position, rotation, GameManager.BulletsParent.transform);
        BulletController bulletController = createdBullet.GetComponent<BulletController>();
        bulletController.InitializeValues(ProjectileStrength, currentTarget.position, transform, currentTarget, BulletSpeed);
    }
}
