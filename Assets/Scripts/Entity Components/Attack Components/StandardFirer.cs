using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFirer : EntityComponent {

    [SerializeField]
    float fireCooldown;
    [SerializeField]
    Transform bulletsParent;
    [SerializeField]
    Transform projectile;
    [SerializeField]
    float arcOfFire;
    [SerializeField]
    float maximumDistanceOfFire;

    float currentFireTimer = 0f;
    
    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Deaggro, OnDeaggro);
        entityEmitter.SubscribeToEvent(EntityEvents.Hurt, OnHurt);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Recovered, OnRecovered);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Hurt, OnHurt);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Recovered, OnRecovered);
    }

    #region EntityEvent handlers

    void OnUpdate()
    {
        if (currentFireTimer <= 0f)
        {
            Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
            Vector3 directionToTarget = currentTarget.position - transform.position;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            float squaredDistanceToTarget = directionToTarget.sqrMagnitude;

            if (Mathf.Abs(angleToTarget) <= arcOfFire && squaredDistanceToTarget <= maximumDistanceOfFire * maximumDistanceOfFire)
            {
                FireProjectile(currentTarget);
                currentFireTimer = fireCooldown; 
            }
        }
        else if (currentFireTimer > 0f)
        {
            currentFireTimer -= Time.deltaTime;
        }
    }

    void OnAggro()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }
    
    void OnDeaggro()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnHurt()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnDead()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnRecovered()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    #endregion

    void FireProjectile(Transform currentTarget)
    {
        Vector3 relativePos = currentTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        Transform createdBullet = Object.Instantiate(projectile, transform.position, rotation);
        createdBullet.transform.parent = bulletsParent.transform;
    }
}
