using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiposteHardware : MonoBehaviour, IHardware {
    // Naming: "Dash" refers to the short Blink triggered when a bullet hits the player while the player is channeling.
    // "Riposte" refers both to the gear itself and to the teleport/damage combo triggered if the player impacts an
    // entity while Dashing.

    HardwareTypes type = HardwareTypes.Riposte;
    public HardwareTypes Type { get { return type; } }

    HardwareUseTypes hardwareUseType = HardwareUseTypes.Channel;
    public HardwareUseTypes HardwareUseType { get { return hardwareUseType; } }

    EntityGearManagement gear;

    int baseStaminaCost = 10;
    public int BaseStaminaCost { get { return baseStaminaCost; } }
    public int StaminaCost { get { return baseStaminaCost; } }

    float riposteDamage = 100f;
    float initialDashRange = 10f;
    // In units per second.
    float dashRangeIncreaseRate = 8f;

    Vector3 currentDashAimPosition;

    bool isChanneling = false;
    public bool IsInUse { get { return isChanneling; } }

    bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float percentOfCooldownRemaining = 0.0f;
    public CooldownDelegate CooldownUpdater { get; set; }

    bool hasDashed = false;

    bool isDashing = false;
    public bool IsDashing { get { return isDashing; } }

    float riposteCooldown = 4f;

    float hangTimeBeforeDashStarts = 0.1f;
    float timeToCompleteDash = 0.4f;
    float timeToCompleteRipsote = 0.15f;

    float timeToAbsorbBullet = 0.4f;

    GameObject riposteZone;
    const string RIPOSTE_ZONE_PATH = "Prefabs/Abilities/RiposteZone";

    MobileEntityHealthComponent healthComponent;
    Material originalSkin;

    Renderer entityRenderer;
    ManticoreInputComponent inputComponent;
    ManticoreAudioComponent audioComponent;

    Material blinkSkin;
    Collider entityCollider;
    TrailRenderer trailRenderer;

    private void OnEnable()
    {
        GameObject riposteZonePrefab = (GameObject)Resources.Load(RIPOSTE_ZONE_PATH);
        riposteZone = Instantiate(riposteZonePrefab, transform);

        gear = GetComponent<EntityGearManagement>();
        healthComponent = GetComponent<MobileEntityHealthComponent>();

        entityRenderer = GetComponent<Renderer>();
        inputComponent = GetComponent<ManticoreInputComponent>();
        audioComponent = GetComponent<ManticoreAudioComponent>();

        blinkSkin = riposteZone.GetComponent<Renderer>().material;
        entityCollider = GetComponent<Collider>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnDestroy()
    {
        Destroy(riposteZone);
    }

    public void UseActiveHardware()
    {
        if (isChanneling)
        {
            riposteZone.SetActive(false);
            StopChanneling();
        }
        else
        {
            BeginChanneling();
        }
    }


    #region Active riposte logic
    void BeginChanneling()
    {
        StartCoroutine(EnterReadyState());
    }

    void StopChanneling()
    {
        isChanneling = false;
        hasDashed = false;

        GameManager.DeactivateGearRangeIndicator();

        StartCoroutine(GoOnCooldown());
    }

    IEnumerator GoOnCooldown()
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < riposteCooldown)
        {
            timeElapsed += Time.deltaTime;
            percentOfCooldownRemaining = 1 - (timeElapsed / riposteCooldown);
            CooldownUpdater(percentOfCooldownRemaining);
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        CooldownUpdater(percentOfCooldownRemaining);
        isOnCooldown = false;
    }

    IEnumerator EnterReadyState()
    {
        isOnCooldown = true;
        isChanneling = true;
        riposteZone.SetActive(true);

        GearRangeIndicator gearRangeIndicator = GameManager.ActivateAndGetGearRangeIndicator();
        float currentRiposteRange = initialDashRange;

        healthComponent.IsInvulnerable = true;

        while (isChanneling && !hasDashed)
        {
            currentRiposteRange += dashRangeIncreaseRate * Time.deltaTime;

            Vector3 mousePosition = GameManager.GetMousePositionOnPlayerPlane();
            Vector3 normalizedToMouse = (mousePosition - transform.position).normalized;
            currentDashAimPosition = (normalizedToMouse * currentRiposteRange) + transform.position;

            gearRangeIndicator.UpdatePosition(currentDashAimPosition);

            yield return null;
        }
    }

    public void StartAbsorbingBullet(GameObject bullet)
    {
        if (!hasDashed)
        {
            hasDashed = true;
            StartCoroutine(AbsorbBullet(bullet, true));
        }
        else
        {
            StartCoroutine(AbsorbBullet(bullet, false));
        }
    }

    IEnumerator AbsorbBullet(GameObject bullet, bool ripostingBullet)
    {
        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;

        Renderer bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.material = blinkSkin;

        Vector3 initialSize = bullet.transform.lossyScale;
        Vector3 destinationSize = Vector3.zero;

        Vector3 initialPosition = bullet.transform.position;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToAbsorbBullet)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / timeToAbsorbBullet;

            bullet.transform.position = Vector3.Lerp(initialPosition, transform.position, percentageComplete);
            bullet.transform.localScale = Vector3.Lerp(initialSize, destinationSize, percentageComplete);

            yield return null;
        }

        if (ripostingBullet)
        {
            BeginDash();
        }
        Destroy(bullet.gameObject);
    }

    void BeginDash()
    {
        StopChanneling();
        StartCoroutine("Dash");
    }

    IEnumerator Dash()
    {
        inputComponent.LockActions(true);
        inputComponent.LockMovement(true);

        healthComponent.IsInvulnerable = true;
        originalSkin = entityRenderer.material;
        entityRenderer.material = blinkSkin;
        entityCollider.enabled = false;

        yield return new WaitForSeconds(hangTimeBeforeDashStarts);

        trailRenderer.enabled = true;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
        Vector3 destinationPosition = currentDashAimPosition;
        isDashing = true;

        float timeElapsed = 0.0f;
        AnimationCurve blinkCurve = GameManager.BlinkCompletionCurve;

        while (timeElapsed < timeToCompleteDash)
        {
            float percentageComplete = timeElapsed / timeToCompleteDash;

            float curveEvaluation = blinkCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(initialPosition, destinationPosition, curveEvaluation);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        trailRenderer.enabled = false;
        inputComponent.LockActions(false);
        inputComponent.LockMovement(false);
        healthComponent.IsInvulnerable = false;
        entityCollider.enabled = true;
        entityRenderer.material = originalSkin;
        isDashing = false;
        riposteZone.SetActive(false);
    }

    public void BeginRiposte(Transform target)
    {
        riposteZone.SetActive(false);
        isDashing = false;
        StopCoroutine("Dash");
        StartCoroutine(Riposte(target));
    }

    IEnumerator Riposte(Transform target)
    {
        EntityEmitter targetEmitter = target.GetComponent<EntityEmitter>();

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float distanceBehindTarget = target.GetComponent<Collider>().bounds.extents.z;
        Vector3 destinationPositionLocalToTarget = new Vector3(0f, 0f, -distanceBehindTarget);

        float timeElapsed = 0.0f;
        AnimationCurve blinkCurve = GameManager.BlinkCompletionCurve;
        targetEmitter.EmitEvent(EntityEvents.FreezeRotation);

        while (timeElapsed < timeToCompleteRipsote)
        {
            float percentageComplete = timeElapsed / timeToCompleteRipsote;

            Vector3 targetPosition = target.TransformPoint(destinationPositionLocalToTarget);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, percentageComplete);
            transform.rotation = Quaternion.Lerp(initialRotation, target.rotation, percentageComplete);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        entityRenderer.material = originalSkin;

        yield return new WaitForSeconds(0.2f);

        GameManager.FreezeGame(GlobalConstants.GameFreezeEvent.Riposte);
        target.GetComponent<MobileEntityHealthComponent>().ReceiveDamageDirectly(transform, riposteDamage);
        gear.ApplyPassiveHardware(typeof(RiposteHardware), target.gameObject);
        if (audioComponent != null)
        {
            audioComponent.PlayGearSound(HardwareTypes.Riposte);
        }
        targetEmitter.EmitEvent(EntityEvents.ResumeRotation);

        trailRenderer.enabled = false;
        healthComponent.IsInvulnerable = false;
        entityCollider.enabled = true;
        inputComponent.LockActions(false);
        inputComponent.LockMovement(false);
    }
    #endregion

    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        switch (activeHardwareType)
        {
            case HardwareTypes.Parry:
                ApplyPassiveHardware_Parry(subject);
                break;
            case HardwareTypes.Blink:
                ApplyPassiveHardware_Blink(activeHardware as BlinkHardware, subject);
                break;
            case HardwareTypes.Nullify:
                ApplyPassiveHardware_Nullify(subject);
                break;
            case HardwareTypes.Riposte:
                Debug.LogError("Trying to apply Riposte passive hardware to Riposte active hardware.");
                break;
            default:
                break;
        }
    }

    void ApplyPassiveHardware_Parry(GameObject bullet)
    {
        bullet.GetComponent<BasicBullet>().IsHoming = true;
    }

    void ApplyPassiveHardware_Blink(BlinkHardware blinkHardware, GameObject entity)
    {
        blinkHardware.DoesBlinkDamage = true;
        blinkHardware.BlinkDamage = riposteDamage / 3f;
    }

    void ApplyPassiveHardware_Nullify(GameObject nullifyZone)
    {
        nullifyZone.GetComponent<Collider>().isTrigger = true;
        nullifyZone.GetComponent<Nullify>().IsReflecting = true;
    }
}
