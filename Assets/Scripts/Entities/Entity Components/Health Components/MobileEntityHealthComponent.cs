using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MobileEntityHealthComponent : EntityComponent {

    EntityData entityData { get { return entityInformation.Data; } }

    float initialHealth { get { return entityData.Health; } }
    GlobalConstants.EntityAllegiance entityAllegiance { get { return entityInformation.Data.Allegiance; } }
    bool isPlayer { get { return entityInformation.Data.isPlayer; } }

    float recoveryTime = 0.8f;
    float timeToDie = 1.0f;
    [SerializeField]
    Material deadSkin;
    [SerializeField]
    Material damageFlashMaterial;
    [SerializeField]
    Material deathFlashMaterial;

    [SerializeField]
    Transform floatingDamageTextPrefab;
    [SerializeField]
    GameObject unitHealthBarPrefab;

    GameObject unitHealthBarObject;
    UnitHealthBar unitHealthBar;

    Renderer[] renderers;
    int renderersCount;
    Material[] defaultMaterials;
    Camera mainCamera;

    float currentRecoveryTimer;
    float currentDeathTimer;
    Material originalSkin;

    float currentHealth = -1;
    [HideInInspector]
    public bool IsInvulnerable = false;
    bool isStunned = false;
    bool isDead = false;

    #region accessors
    public float CurrentHealth()
    {
        if (currentHealth == -1)
        {
            currentHealth = initialHealth;
        }
        return currentHealth;
    }

    public float InitialHealth()
    {
        return initialHealth;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        renderers = GetComponentsInChildren<Renderer>();
        renderersCount = renderers.Length;
        defaultMaterials = new Material[renderersCount];

        for (int i = 0; i < renderersCount; i++)
        {
            defaultMaterials[i] = renderers[i].material;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        mainCamera = Camera.main;
        currentHealth = initialHealth;

        // Instantiate attached health bar, assign values, then hide.
        if (unitHealthBar == null)
        {
            Vector3 healthBarPosition = mainCamera.WorldToScreenPoint(transform.position);
            healthBarPosition.x -= 30f;
            healthBarPosition.y -= 30f;
            Transform unitHealthBarParent = GameManager.HUD.transform.Find("Unit Health Bars");
            unitHealthBarObject = Instantiate(unitHealthBarPrefab, healthBarPosition, Quaternion.identity, unitHealthBarParent);
            unitHealthBar = unitHealthBarObject.GetComponent<UnitHealthBar>();
            unitHealthBar.attachedUnit = transform;
            unitHealthBar.SetTotalHealth(initialHealth);
        }

        unitHealthBarObject.SetActive(false);
    }

    protected override void Subscribe() {
        entityEmitter.SubscribeToEvent(EntityEvents.Invulnerable, OnInvulnerable);
        entityEmitter.SubscribeToEvent(EntityEvents.Vulnerable, OnVulnerable);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnStun);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnUnstun);
	}

    protected override void Unsubscribe() {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Invulnerable, OnInvulnerable);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Vulnerable, OnVulnerable);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnStun);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnUnstun);
	}

    void OnInvulnerable()
    {
        IsInvulnerable = true; 
    }

    void OnVulnerable()
    {
        IsInvulnerable = false; 
    }

    void OnStun()
    {
        isStunned = true;
    }

    void OnUnstun()
    {
        isStunned = false;
    }

    void LowerHealthAmount(float amountToLower)
    {
        currentHealth -= amountToLower;

        if (isPlayer)
        {
            float healthPercentage = currentHealth / initialHealth;
            if (!GameManager.IsPlayerLowHealth && healthPercentage <= 0.2f)
            {
                GameManager.SetIsPlayerLowHealth(true);
            }
        }
    }

    void RaiseHealthAmount(float amountToRaise)
    {
        currentHealth += amountToRaise;

        if (currentHealth > initialHealth)
        {
            currentHealth = initialHealth;
        }

        if (isPlayer)
        {
            float healthPercentage = currentHealth / initialHealth;
            if (GameManager.IsPlayerLowHealth && healthPercentage > 0.2f)
            {
                GameManager.SetIsPlayerLowHealth(false);
            }
        }

        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
    }

    public void OnCollisionEnter(Collision projectile)
    {
        if (DoesBulletDamage(projectile.gameObject) && !IsInvulnerable) 
        {
            // Get & deal damage.
            BulletController bullet = projectile.transform.GetComponent<BulletController>();
            float damage = bullet.strength;
            TakeDamage(damage);

            if (currentHealth > 0)
            {
                RespondToDamage(projectile.relativeVelocity);
            }
            else
            {
                RespondToDeath(projectile.relativeVelocity);
            }
		}
    }

    public void ReceiveDamageDirectly(Transform damagingEntity, float damage)
    {
        TakeDamage(damage);

        if (currentHealth > 0)
        {
            RespondToDamage(damagingEntity);
        }
        else
        {
            RespondToDeath(damagingEntity);
        }
    }

    public void GetHealed(float healing)
    {
        RaiseHealthAmount(healing);

        // Trigger floating damage text.
        Vector3 damageTextPosition = mainCamera.WorldToScreenPoint(transform.position);
        damageTextPosition.y += 15f;
        Transform instantiatedDamageText = Instantiate(floatingDamageTextPrefab, damageTextPosition, Quaternion.identity, GameManager.HUD.transform);
        FloatingDamageText floatingDamageText = instantiatedDamageText.GetComponent<FloatingDamageText>();
        floatingDamageText.isHealing = true;
        floatingDamageText.DamageValue = healing;
        floatingDamageText.attachedTransform = transform;

        // Expose & update attached health bar.
        unitHealthBarObject.SetActive(true);
        unitHealthBar.UpdateHealth(currentHealth);
    }
    
    void TakeDamage(float damage)
    {
        damage *= entityStats.GetDamageReceivedModifier();
        LowerHealthAmount(damage);

        // Trigger floating damage text.
        Vector3 damageTextPosition = mainCamera.WorldToScreenPoint(transform.position);
        damageTextPosition.y += 15f;
        Transform instantiatedDamageText = Instantiate(floatingDamageTextPrefab, damageTextPosition, Quaternion.identity, GameManager.HUD.transform);
        FloatingDamageText floatingDamageText = instantiatedDamageText.GetComponent<FloatingDamageText>();
        floatingDamageText.DamageValue = damage;
        floatingDamageText.attachedTransform = transform;

        // Expose & update attached health bar.
        unitHealthBarObject.SetActive(true);
        unitHealthBar.UpdateHealth(currentHealth);

        // Player becomes invulnerable on damage
        if (isPlayer)
        {
            Invoke("SetInvulnerable", recoveryTime - 0.1f);
            GameManager.ShakeScreen();
        }
        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
    }

    // Used to slightly delay invulnerability so blasts go through.
    void SetInvulnerable()
    {
        CancelInvoke();
        IsInvulnerable = true;
    }

    bool DoesBulletDamage(GameObject bullet)
    {
        if (entityAllegiance == GlobalConstants.EntityAllegiance.Enemy)
        {
            return bullet.CompareTag("FriendlyBullet");
        }
        else if (entityAllegiance == GlobalConstants.EntityAllegiance.Friendly)
        {
            return bullet.CompareTag("EnemyBullet");
        }
        else
        {
            return false; 
        }
    }

    void RespondToDamage(Transform damagingEntity)
    {
        Vector3 normalizedAwayFromEntity = (transform.position - damagingEntity.position).normalized;
        Vector3 scaledAwayFromEntity = normalizedAwayFromEntity * 60f;
        RespondToDamage(scaledAwayFromEntity);
    }

    void RespondToDeath(Transform damagingEntity)
    {
        Vector3 normalizedAwayFromEntity = (transform.position - damagingEntity.position).normalized;
        Vector3 scaledAwayFromEntity = normalizedAwayFromEntity * 60f;
        RespondToDeath(scaledAwayFromEntity);
    }

    void RespondToDamage(Vector3 damagingProjectileCollisionVelocity)
    {
        GameManager.FreezeGame(GlobalConstants.GameFreezeEvent.EntityInjured);
        // Announce hurt; subscribe to handle timer, lerping material, etc.
        entityEmitter.EmitEvent(EntityEvents.Hurt);
        if (!isPlayer)
        {
            if (!isStunned)
            {
                //entityEmitter.EmitEvent(EntityEvents.Stun);
            }

            // Knock back
            Vector3 collisionVelocity = damagingProjectileCollisionVelocity;
            entityInformation.EntityRigidbody.velocity = collisionVelocity;

            entityInformation.EntityRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            entityInformation.EntityRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(collisionVelocity.x * collisionVelocity.z)), 0f);
        }

        // Initialize timer from set values
        currentRecoveryTimer = recoveryTime;

        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = damageFlashMaterial;
        }

        StartCoroutine("HandleDamage");
    }

    void RespondToDeath(Vector3 killingProjectileCollisionVelocity)
    {
        isDead = true;
        GameManager.FreezeGame(GlobalConstants.GameFreezeEvent.EntityDead);
		entityEmitter.EmitEvent(EntityEvents.Dead);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != false)
        {
            agent.enabled = false;
        }

        // Knock back
        Vector3 collisionVelocity = killingProjectileCollisionVelocity;

        gameObject.layer = LayerMask.NameToLayer("DeadEntity");
        entityInformation.EntityRigidbody.isKinematic = false;
        entityInformation.EntityRigidbody.useGravity = true;
        entityInformation.EntityRigidbody.drag = 1f;
        entityInformation.EntityRigidbody.constraints = RigidbodyConstraints.None;
        entityInformation.EntityRigidbody.AddForce(collisionVelocity, ForceMode.Impulse);
        entityInformation.EntityRigidbody.AddTorque(collisionVelocity.z, 0f, -collisionVelocity.x, ForceMode.Impulse);

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].useGravity = true;
        }

        Animator animator = GetComponent<Animator>();

        // Initialize timer from set values
        currentDeathTimer = timeToDie;

        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = deathFlashMaterial;

        }

        StartCoroutine("HandleDeath");

    }

    IEnumerator HandleDamage()
    {
        while (true)
        {
            if (currentRecoveryTimer > 0f)
            {
                // Lerp material while entity is recovering.
                // TODO: Distinguish recovery period more clearly.
                float skinTransitionCompletion = (recoveryTime - currentRecoveryTimer) / recoveryTime;

                currentRecoveryTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else if (!isDead)
            {
                // Once entity has recovered, disable physics and resume action.
                for (int i = 0; i < renderersCount; i++)
                {
                    renderers[i].material = defaultMaterials[i];
                }
                entityInformation.EntityRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                if (isStunned)
                {
                    entityEmitter.EmitEvent(EntityEvents.Unstun);
                }
                // Revert invulnerable-on-damage
                if (isPlayer)
                {
                    IsInvulnerable = false;
                }
                yield break;
            }
            else
            {
                yield break;
            }
        }
    }

    IEnumerator HandleDeath()
    {
        while (true)
        {
            if (currentDeathTimer > 0f)
            {
                float skinTransitionCompletion = (timeToDie - currentDeathTimer) / timeToDie;
                skinTransitionCompletion = Mathf.Sqrt(skinTransitionCompletion);
                for (int i = 0; i < renderersCount; i++)
                {
                    renderers[i].material.Lerp(defaultMaterials[i], deadSkin, skinTransitionCompletion);
                }

                currentDeathTimer -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                unitHealthBar.enabled = false;
                for (int i = 0; i < renderersCount; i++)
                {
                    renderers[i].material = deadSkin;
                }
                entityEmitter.isMuted = true;
                yield break;
            }
        }
    }

}
