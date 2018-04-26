using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StationaryEntityHealthComponent : EntityComponent {

    EntityData entityData { get { return entityInformation.Data; } }

    float initialHealth { get { return entityData.Health; } }
    GlobalConstants.EntityAllegiance entityAllegiance { get { return entityInformation.Data.Allegiance; } }

    float recoveryTime = 0.6f;
    float invulnLag = 0.1f;
    float invulnTime = 2f;
    float timeToDie = 1.0f;
    float timeToSinkOnDeath = 3f;
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

    const string PLAYER_LAYER = "Player";
    const string DEAD_ENTITY = "DeadEntity";
    const string IGNORE_ALL = "IgnoreAll";

    float currentHealth = -1;
    [HideInInspector]
    public bool IsInvulnerable = false;
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
        entityEmitter.SubscribeToEvent(EntityEvents.Respawning, OnRespawn);
	}

    protected override void Unsubscribe() {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Respawning, OnRespawn);
	}

    void OnRespawn()
    {
        isDead = false;
        IsInvulnerable = false;
        currentHealth = initialHealth;

        entityEmitter.EmitEvent(EntityEvents.HealthChanged);

        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = defaultMaterials[i];
        }           

        unitHealthBar.enabled = true;

        entityInformation.EntityRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        entityInformation.EntityRigidbody.useGravity = false;
        entityInformation.EntityRigidbody.isKinematic = false;

        gameObject.layer = LayerMask.NameToLayer(PLAYER_LAYER);
    }

    void LowerHealthAmount(float amountToLower)
    {
        currentHealth -= amountToLower;
    }

    void RaiseHealthAmount(float amountToRaise)
    {
        currentHealth += amountToRaise;

        if (currentHealth > initialHealth)
        {
            currentHealth = initialHealth;
        }

        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
    }

    public void OnCollisionEnter(Collision projectile)
    {
        if (DoesBulletDamage(projectile.gameObject) && !IsInvulnerable) 
        {
            // Get & deal damage.
            BulletController bullet = projectile.transform.GetComponent<BulletController>();
            float damage = bullet.Strength;
            TakeDamage(damage);

            if (currentHealth > 0)
            {
                RespondToDamage();
            }
            else
            {
                RespondToDeath();
            }
		}
    }

    public void ReceiveDamageDirectly(Transform damagingEntity, float damage)
    {
        if (IsInvulnerable)
        {
            return;
        }
        TakeDamage(damage);

        if (currentHealth > 0)
        {
            RespondToDamage();
        }
        else
        {
            RespondToDeath();
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
        if (IsInvulnerable)
        {
            return;
        }
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

        entityEmitter.EmitEvent(EntityEvents.HealthChanged);
    }

    // Used to slightly delay invulnerability so blasts go through.
    void SetInvulnerable()
    {
        CancelInvoke();
        IsInvulnerable = true;

        Invoke("SetVulnerable", invulnTime);
    }

    void SetVulnerable()
    {
        IsInvulnerable = false;
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

    void RespondToDamage()
    {
        GameManager.FreezeGame(GlobalConstants.GameFreezeEvent.EntityInjured);
        // Announce hurt; subscribe to handle timer, lerping material, etc.
        entityEmitter.EmitEvent(EntityEvents.Hurt);

        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = damageFlashMaterial;
        }

        StartCoroutine("HandleDamage");
    }

    void RespondToDeath()
    {
        isDead = true;
        GameManager.FreezeGame(GlobalConstants.GameFreezeEvent.EntityDead);
		entityEmitter.EmitEvent(EntityEvents.Dead);

        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = deathFlashMaterial;

        }

        StartCoroutine("HandleDeath");

    }

    IEnumerator HandleDamage()
    {
        float recoveredTime = Time.time + recoveryTime;

        while (Time.time < recoveredTime)
        {
            yield return null;
        }

        if (!isDead)
        {
            // Once entity has recovered, disable physics and resume action.
            for (int i = 0; i < renderersCount; i++)
            {
                renderers[i].material = defaultMaterials[i];
            }
        }

        yield break;
    }

    IEnumerator HandleDeath()
    {
        float deathTransitionCompletionTime = Time.time + timeToDie;
        GetComponent<Collider>().enabled = false;

        while (Time.time < deathTransitionCompletionTime)
        {
            float skinTransitionCompletion = (deathTransitionCompletionTime - Time.time) / timeToDie;
            skinTransitionCompletion = 1 - skinTransitionCompletion;
            for (int i = 0; i < renderersCount; i++)
            {
                renderers[i].material.Lerp(deathFlashMaterial, deadSkin, skinTransitionCompletion);
            }
            yield return null;
        }

           
        unitHealthBar.enabled = false;
        for (int i = 0; i < renderersCount; i++)
        {
            renderers[i].material = deadSkin;
        }
        entityEmitter.isMuted = true;

        entityInformation.EntityRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        entityInformation.EntityRigidbody.isKinematic = true;
        entityInformation.EntityRigidbody.useGravity = false;

        gameObject.layer = LayerMask.NameToLayer(IGNORE_ALL);

        Vector3 startingPosition = transform.position;
        Vector3 destinationPosition = startingPosition;
        destinationPosition.y -= entityInformation.EntityCollider.bounds.size.y;
        float timeSunk = Time.time + timeToSinkOnDeath;

        while (Time.time < timeSunk)
        {
            float sinkingCompletion = (timeSunk - Time.time) / timeToSinkOnDeath;
            sinkingCompletion = 1 - sinkingCompletion;

            Vector3 newPosition = Vector3.Lerp(startingPosition, destinationPosition, sinkingCompletion);
            transform.position = newPosition;

            yield return null;
        }

        Destroy(gameObject);

        yield break;
    }
    
}
