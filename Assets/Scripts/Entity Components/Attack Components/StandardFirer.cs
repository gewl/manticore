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
	[SerializeField]
    float bulletSpeed;

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
        FireProjectile(currentTarget);
    }

    #endregion

    void FireProjectile(Transform currentTarget)
    {
        Vector3 relativePos = currentTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        Transform createdBullet = Object.Instantiate(projectile, firer.position, rotation);
        BasicBullet bulletController = createdBullet.GetComponent<BasicBullet>();
        bulletController.firer = transform;
        bulletController.target = currentTarget;
        bulletController.speed = bulletSpeed;

        createdBullet.transform.parent = bulletsParent.transform;
    }
}
