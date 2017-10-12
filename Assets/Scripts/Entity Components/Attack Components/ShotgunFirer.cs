using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunFirer : EntityComponent
{

    [SerializeField]
    Transform bulletsParent;
    [SerializeField]
    Transform projectile;
    [SerializeField]
    float projectileStrength = 20f;
    [SerializeField]
    int numberOfProjectilesFired = 3;
    [SerializeField]
    float arcOfFire = 20f;
    [SerializeField]
    Transform firer;
    [SerializeField]
    float bulletSpeed = 30f;

    enum FireType { Direct, Lead }
    [SerializeField]
    FireType currentFireType = FireType.Lead;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
    }

    #region EntityEvent handlers

    void OnPrimaryFire()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        for (int i = 0; i < numberOfProjectilesFired; i++)
        {
            //Invoke("FireProjectile", 0.01f * i);
            FireProjectile();
        }
    }

    #endregion

    void FireProjectile()
    {
        Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
        Vector3 relativePos = currentTarget.position - firer.transform.position;
        if (currentFireType == FireType.Lead)
        {
            float timeToImpact = relativePos.sqrMagnitude / (bulletSpeed * bulletSpeed);
            Vector3 currentTargetVelocity = currentTarget.GetComponent<Rigidbody>().velocity;
            currentTargetVelocity *= timeToImpact;
            relativePos += currentTargetVelocity;
        }
        float angleAdjustment = Random.Range(-arcOfFire, arcOfFire);
        relativePos = Vector3.RotateTowards(relativePos, transform.right, Mathf.Deg2Rad * angleAdjustment, 1);
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
}
