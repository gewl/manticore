using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEntityHealthComponent : EntityComponent {

    [SerializeField]
    int currentHealth;
    [SerializeField]
    float recoveryTime;
    [SerializeField]
    Material damagedSkin;
    [SerializeField]
    Material deadSkin;
    [SerializeField]
    Material flashSkin;

    MeshRenderer meshRenderer;
    float currentRecoveryTimer;
    Material originalSkin;

    bool isInvulnerable = false;

    // This is a weird one. For the time being, functionality is triggered by bullet entering,
    // so no need to actually subscribe it to anything on Initialize.
    protected override void Subscribe() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Unsubscribe() { }

    void OnFixedUpdate()
    {
        if (currentRecoveryTimer > 0f)
        {
            float skinTransitionCompletion = (recoveryTime - currentRecoveryTimer) / recoveryTime;
            skinTransitionCompletion = Mathf.Sqrt(1 - skinTransitionCompletion);
            meshRenderer.material.Lerp(originalSkin, damagedSkin, skinTransitionCompletion);

            currentRecoveryTimer -= Time.deltaTime;
        }
        else
        {
            meshRenderer.material = damagedSkin;
            entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            entityEmitter.EmitEvent(EntityEvents.Recovered);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
            StartCoroutine("Flash");
        }
    }

    public void OnTriggerEnter(Collider projectile)
    {
        if (projectile.gameObject.tag == "Bullet" && projectile.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(transform)) {
            Debug.Log("Hit by bullet");
			UnityEngine.Object.Destroy(projectile.gameObject);

            if (!isInvulnerable)
            {
                currentHealth--;
                if (currentHealth > 0)
                {
                    Damage(projectile);
                }
                else
                {
                    Die(projectile);
                }
            }
		}
    }

    void Damage(Collider damagingProjectile)
    {
        // Announce hurt; subscribe to handle timer, lerping material, etc.
        entityEmitter.EmitEvent(EntityEvents.Hurt);
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);

        // Knock back
        Vector3 projectileVelocity = damagingProjectile.attachedRigidbody.velocity;
        entityData.EntityRigidbody.velocity = projectileVelocity;

        entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        entityData.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(projectileVelocity.x * projectileVelocity.z)), 0f);

        // Initialize timer from set values
        currentRecoveryTimer = recoveryTime;

        // Store original skin for lerping
        originalSkin = meshRenderer.material;
    }

    void Die(Collider killingProjectile)
    {

    }

    IEnumerator Flash()
    {
        bool hasFlashed = false;
        Material originalMaterial = meshRenderer.material;
        while (true)
        {
            if (!hasFlashed)
            {
                meshRenderer.material = flashSkin;
                hasFlashed = true;
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                meshRenderer.material = originalMaterial;
                yield break;
            }
        }
    }
}
