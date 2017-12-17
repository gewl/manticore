using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyHardware : MonoBehaviour, IHardware {

    HardwareTypes type = HardwareTypes.Nullify;
    public HardwareTypes Type { get { return type; } }

    public bool IsInUse { get { return false; } }

    HardwareUseTypes hardwareUseType = HardwareUseTypes.Instant;
    public HardwareUseTypes HardwareUseType { get { return hardwareUseType; } }

    EntityGearManagement gear;

    private int baseStaminaCost = 80;
    public int BaseStaminaCost { get { return baseStaminaCost; } }
    public int UpdatedStaminaCost { get { return baseStaminaCost; } }

    private bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float nullifyCooldown = 4f;
    float percentOfCooldownRemaining = 0.0f;
    public CooldownDelegate CooldownUpdater { get; set; }

    const string NULLIFY_EMANATE_PATH = "Prefabs/Abilities/NullifierEmanateEffect";
    GameObject _nullifyEmanateEffect;
    GameObject NullifyEmanateEffect
    {
        get
        {
            if (_nullifyEmanateEffect == null)
            {
                _nullifyEmanateEffect = (GameObject)Resources.Load(NULLIFY_EMANATE_PATH);
            }

            return _nullifyEmanateEffect;
        }
    }

    const string NULLIFY_PROJECT_PATH = "Prefabs/Abilities/NullifierProjectEffect";
    GameObject _nullifyProjectEffect;
    GameObject NullifyProjectEffect
    {
        get
        {
            if (_nullifyProjectEffect == null)
            {
                _nullifyProjectEffect = (GameObject)Resources.Load(NULLIFY_PROJECT_PATH);
            }

            return _nullifyProjectEffect;
        }
    }

    // Active hardware values
    float entityNullifyRadius = 8f;
    float timeToCompleteActiveEffect = 0.3f;

    // Passive hardware (Parry) values
    float bulletNullifyRadius = 4f;
    float timeToExpandParryPassiveEffect = 0.1f;
    // Passive hardware (Riposte) values
    float timeToExpandRipostePassiveEffect = 0.8f;
    float hangTimeOnCompletion = 0.2f;

    void OnEnable()
    {
        gear = GetComponent<EntityGearManagement>();
    }

    #region Active hardware use
    public void UseActiveHardware()
    {
        StartCoroutine(FireNullifyEffect(timeToCompleteActiveEffect));
    }

    IEnumerator FireNullifyEffect(float duration, bool shouldFollow = false, bool isActiveHardware = true)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, transform.position, Quaternion.identity);
        if (isActiveHardware)
        {
            gear.ApplyPassiveHardware(typeof(NullifyHardware), spawnedNullification);
        }

        float timeElapsed = 0.0f;
        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(entityNullifyRadius, 1f, entityNullifyRadius);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / duration;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

            spawnedNullification.transform.position = transform.position;
            spawnedNullification.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            yield return null;
        }

        if (isActiveHardware)
        {
            StartCoroutine(GoOnCooldown());
        }
        DestroyObject(spawnedNullification);
        yield break;
    }

    IEnumerator GoOnCooldown()
    {
        float timeElapsed = 0.0f;
        isOnCooldown = true;

        while (timeElapsed < nullifyCooldown)
        {
            timeElapsed += Time.deltaTime;
            percentOfCooldownRemaining = 1 - (timeElapsed / nullifyCooldown);
            CooldownUpdater(percentOfCooldownRemaining);
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        CooldownUpdater(percentOfCooldownRemaining);
        isOnCooldown = false;
    }
    #endregion

    #region Passive hardware use
    public void ApplyPassiveHardware(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        switch (activeHardwareType)
        {
            case HardwareTypes.Parry:
                StartCoroutine(ApplyPassiveHardware_Parry(subject));
                break;
            case HardwareTypes.Blink:
                StartCoroutine(ApplyPassiveHardware_Blink(activeHardware, subject));
                break;
            case HardwareTypes.Nullify:
                Debug.LogError("Trying to apply Nullify passive effect to Nullify active hardware.");
                break;
            case HardwareTypes.Riposte:
                StartCoroutine(ApplyPassiveHardware_Riposte(activeHardware, subject));
                break;
            default:
                break;
        }
    }

    IEnumerator ApplyPassiveHardware_Parry(GameObject bullet)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, bullet.transform.position, Quaternion.identity, bullet.transform);

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(bulletNullifyRadius, 1f, bulletNullifyRadius);

        float timeElapsed = 0.0f;
        while (timeElapsed < timeToExpandParryPassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToExpandParryPassiveEffect;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

            spawnedNullification.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            yield return null;
        }

        yield break;
    }

    IEnumerator ApplyPassiveHardware_Blink(IHardware activeHardware, GameObject player)
    {
        BlinkHardware blinkHardware = (BlinkHardware)activeHardware;

        float blinkDuration = blinkHardware.TimeToCompleteBlink + blinkHardware.HangTimeBeforeBlinkStarts;

        StartCoroutine(FireNullifyEffect(blinkDuration, true, false));

        yield return null;
    }

    IEnumerator ApplyPassiveHardware_Riposte(IHardware activeHardware, GameObject target)
    {
        Vector3 targetRotationEuler = target.transform.rotation.eulerAngles;
        Vector3 projectedRotationEuler = targetRotationEuler;
        projectedRotationEuler.y += 90f;
        GameObject spawnedNullification = Instantiate(NullifyProjectEffect, target.transform.position, Quaternion.Euler(projectedRotationEuler));

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(10f, originalSize.y, 2f);

        float timeElapsed = 0.0f;
        while (timeElapsed < timeToExpandRipostePassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToExpandRipostePassiveEffect;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

            spawnedNullification.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            spawnedNullification.transform.position = target.transform.position;
            yield return null;
        }

        yield return new WaitForSeconds(hangTimeOnCompletion);

        Destroy(spawnedNullification);

        yield break;
    }

    #endregion
}
