using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiposteHardware : MonoBehaviour, IHardware {
    // Naming: "Dash" refers to the short Blink triggered when a bullet hits the player while the player is channeling.
    // "Riposte" refers both to the gear itself and to the teleport/damage combo triggered if the player impacts an
    // entity while Dashing.

    HardwareUseTypes hardwareUseType = HardwareUseTypes.Channel;
    public HardwareUseTypes HardwareUseType { get { return hardwareUseType; } }

    int baseStaminaCost = 10;
    public int BaseStaminaCost { get { return baseStaminaCost; } }
    public int UpdatedStaminaCost { get { return baseStaminaCost; } }

    float riposteDamage = 100f;
    float initialDashRange = 10f;
    // In units per second.
    float dashRangeIncreaseRate = 4f;

    Vector3 currentDashAimPosition;

    bool isChanneling = false;
    public bool IsInUse { get { return isChanneling; } }

    bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }

    bool hasDashed = false;

    bool isDashing = false;
    public bool IsDashing { get { return isDashing; } }

    float riposteCooldown = 1f;

    float hangTimeBeforeDashStarts = 0.1f;
    float timeToCompleteDash = 0.4f;
    float timeToCompleteRipsote = 0.15f;

    float timeToAbsorbBullet = 0.4f;

    GameObject riposteZone;
    const string RIPOSTE_ZONE = "RiposteZone";

    MobileEntityHealthComponent healthComponent;
    Material originalSkin;

    Renderer entityRenderer;
    ManticoreInputComponent inputComponent;
    Material blinkSkin;
    Collider entityCollider;
    TrailRenderer trailRenderer;

    private void OnEnable()
    {
        entityRenderer = GetComponent<Renderer>();
        entityCollider = GetComponent<Collider>();
        trailRenderer = GetComponent<TrailRenderer>();
        inputComponent = GetComponent<ManticoreInputComponent>();
        healthComponent = GetComponent<MobileEntityHealthComponent>();

        riposteZone = transform.Find(RIPOSTE_ZONE).gameObject;
        blinkSkin = riposteZone.GetComponent<Renderer>().material;
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

    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
    {

    }

    void BeginChanneling()
    {
        StartCoroutine(EnterReadyState());
    }

    void StopChanneling()
    {
        isChanneling = false;

        GameManager.DeactivateGearRangeIndicator();

        StartCoroutine(GoOnCooldown());
    }

    IEnumerator GoOnCooldown()
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < riposteCooldown)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

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
            Transform bulletFirer = bullet.GetComponent<BasicBullet>().firer;
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
        StopChanneling();
        riposteZone.SetActive(false);
    }

    public void BeginRiposte(Transform target)
    {
        StopChanneling();
        riposteZone.SetActive(false);
        isDashing = false;
        StopCoroutine("Dash");
        StartCoroutine(Riposte(target));
    }

    IEnumerator Riposte(Transform target)
    {
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float distanceBehindTarget = target.GetComponent<Collider>().bounds.extents.z + entityCollider.bounds.size.z;
        Vector3 destinationPositionLocalToTarget = new Vector3(0f, 0f, -distanceBehindTarget);

        float timeElapsed = 0.0f;
        AnimationCurve blinkCurve = GameManager.BlinkCompletionCurve;

        while (timeElapsed < timeToCompleteDash)
        {
            float percentageComplete = timeElapsed / timeToCompleteDash;

            Vector3 targetPosition = target.TransformPoint(destinationPositionLocalToTarget);
            float curveEvaluation = blinkCurve.Evaluate(percentageComplete);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, curveEvaluation);
            transform.rotation = Quaternion.Lerp(initialRotation, target.rotation, curveEvaluation);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        target.GetComponent<MobileEntityHealthComponent>().ReceiveDamageDirectly(transform, riposteDamage);
        trailRenderer.enabled = false;
        inputComponent.LockActions(false);
        inputComponent.LockMovement(false);
        healthComponent.IsInvulnerable = false;
        entityCollider.enabled = true;
        entityRenderer.material = originalSkin;
    }

    //IEnumerator Dash(Transform bulletFirer)
    //{
    //    inputComponent.LockActions(true);

    //    inputComponent.LockMovement(true);

    //    healthComponent.IsInvulnerable = true;
    //    Material originalSkin = entityRenderer.material;
    //    entityRenderer.material = blinkSkin;
    //    entityCollider.enabled = false;

    //    yield return new WaitForSeconds(hangTimeBeforeDashStarts);

    //    trailRenderer.enabled = true;

    //    Vector3 initialPosition = transform.position;
    //    Quaternion initialRotation = transform.rotation;

    //    float distanceBehindTarget = bulletFirer.GetComponent<Collider>().bounds.extents.z + entityCollider.bounds.size.z;
    //    Vector3 destinationPositionLocalToTarget = new Vector3(0f, 0f, -distanceBehindTarget);

    //    float timeElapsed = 0.0f;
    //    AnimationCurve blinkCurve = GameManager.BlinkCompletionCurve;

    //    while (timeElapsed < timeToCompleteDash)
    //    {
    //        float percentageComplete = timeElapsed / timeToCompleteDash;

    //        Vector3 targetPosition = bulletFirer.TransformPoint(destinationPositionLocalToTarget);
    //        float curveEvaluation = blinkCurve.Evaluate(percentageComplete);

    //        transform.position = Vector3.Lerp(initialPosition, targetPosition, curveEvaluation);
    //        transform.rotation = Quaternion.Lerp(initialRotation, bulletFirer.rotation, curveEvaluation);

    //        timeElapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    bulletFirer.GetComponent<MobileEntityHealthComponent>().ReceiveDamageDirectly(transform, riposteDamage);
    //    trailRenderer.enabled = false;
    //    inputComponent.LockActions(false);
    //    inputComponent.LockMovement(false);
    //    healthComponent.IsInvulnerable = false;
    //    entityCollider.enabled = true;
    //    entityRenderer.material = originalSkin;
    //}
}
