using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileEntityHealthComponent : EntityComponent {

    [SerializeField]
    float currentHealth;
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

    [SerializeField]
    Transform floatingDamageText;
    [SerializeField]
    Transform hud;
    [SerializeField]
    GameObject unitHealthBarPrefab;

    GameObject unitHealthBarObject;
    UnitHealthBar unitHealthBar;

    MeshRenderer meshRenderer;
    Camera mainCamera;

    float currentRecoveryTimer;
    float currentDeathTimer;
    Material originalSkin;
    bool isEnabled = false;

    enum Allegiance { Friendly, Enemy }
    [SerializeField]
    Allegiance entityAllegiance = Allegiance.Enemy;
    bool isInvulnerable = false;
    float initialHealth;

    #region accessors
    public float CurrentHealth()
    {
        return currentHealth;
    }

    public float InitialHealth()
    {
        if (initialHealth == 0)
        {
            initialHealth = currentHealth;
        }
        return initialHealth;
    }
    #endregion

    protected void OnEnable()
    {
        mainCamera = Camera.main;
        isEnabled = true;
        initialHealth = currentHealth;

        // Instantiate attached health bar, assign values, then hide.
        if (unitHealthBar == null)
        {
            Vector3 healthBarPosition = mainCamera.WorldToScreenPoint(transform.position);
            healthBarPosition.x -= 30f;
            healthBarPosition.y -= 30f;
            Transform unitHealthBarParent = hud.Find("Unit Health Bars");
            unitHealthBarObject = Instantiate(unitHealthBarPrefab, healthBarPosition, Quaternion.identity, unitHealthBarParent);
            unitHealthBar = unitHealthBarObject.GetComponent<UnitHealthBar>();
            unitHealthBar.attachedUnit = transform;
            unitHealthBar.SetTotalHealth(initialHealth);
        }

        unitHealthBarObject.SetActive(false);
    }

    protected override void Subscribe() {
        meshRenderer = GetComponent<MeshRenderer>();

        entityEmitter.SubscribeToEvent(EntityEvents.Invulnerable, OnInvulnerable);
        entityEmitter.SubscribeToEvent(EntityEvents.Vulnerable, OnVulnerable);
	}

    protected override void Unsubscribe() {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Invulnerable, OnInvulnerable);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Vulnerable, OnVulnerable);
	}

    void OnInvulnerable()
    {
        isInvulnerable = true; 
    }

    void OnVulnerable()
    {
        isInvulnerable = false; 
    }

    public void OnCollisionEnter(Collision projectile)
    {
        if (!isEnabled)
        {
            return;
        }
        if (DoesBulletDamage(projectile.gameObject) && !isInvulnerable) 
        {
            // Get & deal damage.
            BasicBullet bullet = projectile.transform.GetComponent<BasicBullet>();
            float damage = bullet.strength;
            currentHealth -= damage;

            // Trigger floating damage text.
            Vector3 damageTextPosition = mainCamera.WorldToScreenPoint(transform.position);
            damageTextPosition.y += 15f;
            Transform instantiatedDamageText = Instantiate(floatingDamageText, damageTextPosition, Quaternion.identity, hud);
            instantiatedDamageText.GetComponent<FloatingDamageText>().DamageValue = damage;

            // Expose & update attached health bar.
            unitHealthBarObject.SetActive(true);
            unitHealthBar.UpdateHealth(currentHealth);

            // Damage or kill depending on remaining health.
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
        entityEmitter.EmitEvent(EntityEvents.Stun);

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
        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
        while (true)
        {
            if (currentRecoveryTimer > 0f)
            {
                // Lerp material while entity is recovering.
                // TODO: Distinguish recovery period more clearly.
                float skinTransitionCompletion = (recoveryTime - currentRecoveryTimer) / recoveryTime;
                meshRenderer.material.Lerp(originalSkin, damagedSkin, skinTransitionCompletion);

                currentRecoveryTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                // Once entity has recovered, disable physics and resume action.
                meshRenderer.material = damagedSkin;
                entityData.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                entityEmitter.EmitEvent(EntityEvents.Unstun);
                isInvulnerable = false;
                yield break;
            }
        }
    }

    IEnumerator DyingProcess()
    {
        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
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
                entityData.EntityRigidbody.AddForce(new Vector3(0f, -100, 0f), ForceMode.Impulse);
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
