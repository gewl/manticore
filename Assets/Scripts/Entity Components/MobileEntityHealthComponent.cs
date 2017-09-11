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
    Material originalSkin;

    bool isInvulnerable = false;

    // This is a weird one. For the time being, functionality is triggered by bullet entering,
    // so no need to actually subscribe it to anything on Initialize.
    protected override void Subscribe() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Unsubscribe() { }

    public void OnTriggerEnter(Collider projectile)
    {
        if (projectile.gameObject.tag == "Bullet" && projectile.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(transform)) {
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

        // Knock back
        Vector3 projectileVelocity = damagingProjectile.attachedRigidbody.velocity;
        entityData.EntityRigidbody.velocity = projectileVelocity;

        entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        entityData.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(projectileVelocity.x * projectileVelocity.z)), 0f);

        // Initialize timer from set values
        currentRecoveryTimer = recoveryTime;

        // Store original skin for lerping
        originalSkin = meshRenderer.material;

        StartCoroutine("DamagedProcess");
    }

    void Die(Collider killingProjectile)
    {
        entityEmitter.EmitEvent(EntityEvents.Dead);

        // Knock back
        Vector3 projectileVelocity = killingProjectile.attachedRigidbody.velocity;
        entityData.EntityRigidbody.velocity = projectileVelocity;

        entityData.EntityRigidbody.constraints = RigidbodyConstraints.None;
        entityData.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(projectileVelocity.x * projectileVelocity.z)), 0f);
        entityData.EntityRigidbody.AddForce(projectileVelocity, ForceMode.Impulse);
        entityData.EntityRigidbody.AddTorque(projectileVelocity.z, 0f, -projectileVelocity.x, ForceMode.Impulse);

        // Initialize timer from set values
        currentRecoveryTimer = recoveryTime;

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
                StartCoroutine("Flash");
                yield break;
            }
        }
    }

    IEnumerator DyingProcess()
    {
        while (true)
        {
            if (currentRecoveryTimer > 0f)
            {
                float skinTransitionCompletion = (recoveryTime - currentRecoveryTimer) / recoveryTime;
                meshRenderer.material.Lerp(originalSkin, deadSkin, skinTransitionCompletion);

                currentRecoveryTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                meshRenderer.material = deadSkin;
                entityData.EntityRigidbody.detectCollisions = false;
                entityData.EntityRigidbody.drag = 10f;
                entityData.EntityRigidbody.freezeRotation = true;
                entityData.EntityRigidbody.AddForce(new Vector3(0f, -100f, 0f));
                StartCoroutine("DarkFlash");
                if (transform.position.y <= -2f)
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
