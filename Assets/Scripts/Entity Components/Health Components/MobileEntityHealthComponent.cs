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
    float timeToDie;
    [SerializeField]
    Material damagedSkin;
    [SerializeField]
    Material deadSkin;
    [SerializeField]
    Material flashSkin;
    [SerializeField]
    Material darkFlashSkin;

    MeshRenderer meshRenderer;
    float currentRecoveryTimer;
    float currentDeathTimer;
    Material originalSkin;

    enum Allegiance { Friendly, Enemy }
    [SerializeField]
    Allegiance entityAllegiance = Allegiance.Enemy;
    bool isInvulnerable = false;

    // This is a weird one. For the time being, functionality is triggered by bullet entering,
    // so no need to actually subscribe it to anything on Initialize.
    protected override void Subscribe() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Unsubscribe() { }

    public void OnCollisionEnter(Collision projectile)
    {
        if (DoesBulletDamage(projectile.gameObject)) {

            if (!isInvulnerable)
            {
                currentHealth--;
                isInvulnerable = true;
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

    bool DoesBulletDamage(GameObject bullet)
    {
        if (entityAllegiance == Allegiance.Enemy)
        {
            return bullet.CompareTag("FriendlyBullet");
        }
        else if (entityAllegiance == Allegiance.Friendly)
        {
            return bullet.CompareTag("EnemyBullet");
        }
        else
        {
            return false; 
        }
    }

    void Damage(Collision damagingProjectileCollision)
    {
        // Announce hurt; subscribe to handle timer, lerping material, etc.
        entityEmitter.EmitEvent(EntityEvents.Hurt);

        // Knock back
        Vector3 collisionVelocity = damagingProjectileCollision.relativeVelocity;
        entityData.EntityRigidbody.velocity = collisionVelocity;

        entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        entityData.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(collisionVelocity.x * collisionVelocity.z)), 0f);

        // Initialize timer from set values
        currentRecoveryTimer = recoveryTime;

        meshRenderer.material = flashSkin;
        // Store original skin for lerping
        originalSkin = meshRenderer.material;

        StartCoroutine("DamagedProcess");
    }

    void Die(Collision killingProjectileCollision)
    {
		entityEmitter.EmitEvent(EntityEvents.Dead);

        // Knock back
        Vector3 collisionVelocity = killingProjectileCollision.relativeVelocity;
        entityData.EntityRigidbody.velocity = collisionVelocity;

        entityData.EntityRigidbody.constraints = RigidbodyConstraints.None;
        collisionVelocity.Normalize();
        entityData.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(collisionVelocity.x * collisionVelocity.z)), 0f);
        entityData.EntityRigidbody.AddForce(collisionVelocity, ForceMode.Impulse);
        entityData.EntityRigidbody.AddTorque(collisionVelocity.z, 0f, -collisionVelocity.x, ForceMode.Impulse);

        // Initialize timer from set values
        currentDeathTimer = timeToDie;

        meshRenderer.material = darkFlashSkin;
        // Store original skin for lerping
        originalSkin = meshRenderer.material;

        StartCoroutine("DyingProcess");

    }

    IEnumerator DamagedProcess()
    {
        while (true)
        {
            if (currentRecoveryTimer > 0f)
            {
                float skinTransitionCompletion = (recoveryTime - currentRecoveryTimer) / recoveryTime;
                meshRenderer.material.Lerp(originalSkin, damagedSkin, skinTransitionCompletion);

                currentRecoveryTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                meshRenderer.material = damagedSkin;
                entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                entityEmitter.EmitEvent(EntityEvents.Recovered);
                isInvulnerable = false;
                yield break;
            }
        }
    }

    IEnumerator DyingProcess()
    {
        while (true)
        {
            if (currentDeathTimer > 0f)
            {
                float skinTransitionCompletion = (timeToDie - currentDeathTimer) / timeToDie;
                skinTransitionCompletion = Mathf.Sqrt(skinTransitionCompletion);
                meshRenderer.material.Lerp(originalSkin, deadSkin, skinTransitionCompletion);

                currentDeathTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                meshRenderer.material = deadSkin;
                entityData.EntityRigidbody.detectCollisions = false;
                entityData.EntityRigidbody.drag = 10f;
                entityData.EntityRigidbody.freezeRotation = true;
                entityData.EntityRigidbody.AddForce(new Vector3(0f, -300, 0f), ForceMode.Impulse);
                if (transform.position.y <= -4f)
                {
                    UnityEngine.Object.Destroy(gameObject);
                }
                yield return null;
            }
        }

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

    IEnumerator DarkFlash()
    {
        bool hasFlashed = false;
        Material originalMaterial = meshRenderer.material;
        while (true)
        {
            if (!hasFlashed)
            {
                meshRenderer.material = darkFlashSkin;
                hasFlashed = true;
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                meshRenderer.material = originalMaterial;
                yield break;
            }
        }
    }
}
