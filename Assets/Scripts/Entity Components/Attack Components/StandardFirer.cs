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
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Aggro, OnAggro);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Deaggro, OnDeaggro);
    }

    void OnUpdate()
    {
        if (currentFireTimer <= 0f)
        {
            Transform currentTarget = (Transform)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentTarget);
            Vector3 directionToTarget = currentTarget.position - transform.position;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            float squaredDistanceToTarget = directionToTarget.sqrMagnitude;
            Debug.Log("ready to fire");

            if (Mathf.Abs(angleToTarget) <= arcOfFire && squaredDistanceToTarget <= maximumDistanceOfFire * maximumDistanceOfFire)
            {
                Debug.Log("firing!");
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

    void FireProjectile(Transform currentTarget)
    {
        Transform createdBullet = Object.Instantiate(projectile, transform.position, transform.rotation);
        createdBullet.transform.parent = bulletsParent.transform;
    }
}
