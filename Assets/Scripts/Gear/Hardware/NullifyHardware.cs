using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyHardware : MonoBehaviour, IHardware {

    EntityGearManagement gear;
    HardwareType type = HardwareType.Nullify;
    public HardwareType Type { get { return type; } }

    public bool IsInUse { get { return false; } }

    HardwareUseType hardwareUseType = HardwareUseType.Instant;
    public HardwareUseType HardwareUseType { get { return hardwareUseType; } }

    NullifyHardwareData subtypeData;
    public void AssignSubtypeData(HardwareData hardwareData)
    {
        subtypeData = hardwareData as NullifyHardwareData;
    }

    int NullifyMomentum { get { return MomentumManager.GetMomentumPointsByHardwareType(HardwareType.Nullify); } } 

    public int StaminaCost { get { return subtypeData.GetStaminaCost(NullifyMomentum); } }
    float NullifyCooldown { get { return subtypeData.GetCooldown(NullifyMomentum); } }

    // Active hardware values
    float NullifyRadius { get { return subtypeData.GetNullifyRadius(NullifyMomentum); } }
    float TimeToExpandActiveEffect { get { return subtypeData.GetTimeToExpand(NullifyMomentum); } }
    float TimeToLinger { get { return subtypeData.GetLingerDuration(NullifyMomentum); } }
    float TotalTimeToComplete { get { return TimeToExpandActiveEffect + TimeToLinger; } }

    // Passive hardware (Parry) values
    float BulletNullifyRadius { get { return 2f; } }
    float TimeToExpandParryPassiveEffect { get { return TimeToExpandActiveEffect / 3f; } }
    // Passive hardware (Riposte) values
    float TimeToExpandRipostePassiveEffect { get { return TimeToExpandActiveEffect * 2f; } }
    float hangTimeOnCompletion = 0.2f;

    private bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
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

    void OnEnable()
    {
        gear = GetComponent<EntityGearManagement>();
    }

    #region Active hardware use
    public void UseActiveHardware()
    {
        StartCoroutine(FireNullifyEffect(TotalTimeToComplete));
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
        Vector3 targetSize = new Vector3(NullifyRadius, 1f, NullifyRadius);

        while (timeElapsed < TimeToExpandActiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / TimeToExpandActiveEffect;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

            spawnedNullification.transform.position = transform.position;
            spawnedNullification.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            yield return null;
        }

        yield return new WaitForSeconds(TimeToLinger);

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

        while (timeElapsed < NullifyCooldown)
        {
            timeElapsed += Time.deltaTime;
            percentOfCooldownRemaining = 1 - (timeElapsed / NullifyCooldown);
            if (CooldownUpdater != null)
            {
                CooldownUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownUpdater != null)
        {
            CooldownUpdater(percentOfCooldownRemaining);
        }
        isOnCooldown = false;
    }
    #endregion

    #region Passive hardware use
    public void ApplyPassiveHardware(HardwareType activeHardwareType, IHardware activeHardware, GameObject subject)
    {
        switch (activeHardwareType)
        {
            case HardwareType.Parry:
                StartCoroutine(ApplyPassiveHardware_Parry(subject));
                break;
            case HardwareType.Blink:
                StartCoroutine(ApplyPassiveHardware_Blink(activeHardware, subject));
                break;
            case HardwareType.Nullify:
                Debug.LogError("Trying to apply Nullify passive effect to Nullify active hardware.");
                break;
            case HardwareType.Fracture:
                StartCoroutine(ApplyPassiveHardware_Fracture(activeHardware, subject));
                break;
            default:
                break;
        }
    }

    IEnumerator ApplyPassiveHardware_Parry(GameObject bullet)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, bullet.transform.position, Quaternion.identity, bullet.transform);

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(BulletNullifyRadius, 1f, BulletNullifyRadius);

        float timeElapsed = 0.0f;
        while (timeElapsed < TimeToExpandParryPassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / TimeToExpandParryPassiveEffect;
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

    IEnumerator ApplyPassiveHardware_Fracture(IHardware activeHardware, GameObject fracturedBullet)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, fracturedBullet.transform.position, Quaternion.identity, fracturedBullet.transform);

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(BulletNullifyRadius, 1f, BulletNullifyRadius);

        float timeElapsed = 0.0f;
        while (timeElapsed < TimeToExpandParryPassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / TimeToExpandParryPassiveEffect;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

            spawnedNullification.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            yield return null;
        }

        yield break;
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
        while (timeElapsed < TimeToExpandRipostePassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / TimeToExpandRipostePassiveEffect;
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
