﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyHardware : MonoBehaviour, IHardware {

    EntityGearManagement gear;
    HardwareType type = HardwareType.Nullify;
    public HardwareType Type { get { return type; } }
    Collider entityCollider;

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
    float BulletNullifyRadius { get { return 3f; } }
    float TimeToExpandParryPassiveEffect { get { return 0.1f; } }
    // Passive hardware (Riposte) values
    float TimeToExpandRipostePassiveEffect { get { return TimeToExpandActiveEffect * 2f; } }
    float hangTimeOnCompletion = 0.2f;

    private bool isOnCooldown = false;
    public bool IsOnCooldown { get { return isOnCooldown; } }
    float percentOfCooldownRemaining = 0.0f;
    public CooldownDelegate CooldownPercentUpdater { get; set; }
    public CooldownDelegate CooldownDurationUpdater { get; set; }

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
        entityCollider = GetComponent<Collider>();
    }

    #region Active hardware use
    public void UseActiveHardware()
    {
        StartCoroutine(FireNullifyEffect(TotalTimeToComplete));
    }

    IEnumerator FireNullifyEffect(float duration, bool shouldFollow = false, bool isActiveHardware = true)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, entityCollider.bounds.center, Quaternion.identity);
        spawnedNullification.GetComponent<Nullify>().TimeToComplete = TotalTimeToComplete;
        if (isActiveHardware)
        {
            gear.ApplyPassiveHardware(typeof(NullifyHardware), spawnedNullification);
        }

        float timeElapsed = 0.0f;
        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(NullifyRadius, 5f, NullifyRadius);

        while (timeElapsed < TimeToExpandActiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / TimeToExpandActiveEffect;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

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
        float timeOffCooldown = Time.time + NullifyCooldown;
        isOnCooldown = true;

        while (Time.time < timeOffCooldown)
        {
            float cooldownRemaining = timeOffCooldown - Time.time;
            percentOfCooldownRemaining = 1 - cooldownRemaining / NullifyCooldown;

            if (CooldownDurationUpdater != null)
            {
                CooldownDurationUpdater(cooldownRemaining);
            }

            if (CooldownPercentUpdater != null)
            {
                CooldownPercentUpdater(percentOfCooldownRemaining);
            }
            yield return null;
        }

        percentOfCooldownRemaining = 0.0f;
        if (CooldownDurationUpdater != null)
        {
            CooldownDurationUpdater(percentOfCooldownRemaining);
        }
        if (CooldownPercentUpdater != null)
        {
            CooldownPercentUpdater(percentOfCooldownRemaining);
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
                StartCoroutine(ApplyNullifyToBullet(subject));
                break;
            case HardwareType.Blink:
                StartCoroutine(ApplyPassiveHardware_Blink(activeHardware, subject));
                break;
            case HardwareType.Nullify:
                Debug.LogError("Trying to apply Nullify passive effect to Nullify active hardware.");
                break;
            case HardwareType.Fracture:
                StartCoroutine(ApplyNullifyToBullet(subject));
                break;
            case HardwareType.Yank:
                ApplyPassiveHardware_Yank(subject);
                break;
            default:
                break;
        }
    }

    IEnumerator ApplyNullifyToBullet(GameObject bullet)
    {
        GameObject spawnedNullification = Instantiate(NullifyEmanateEffect, bullet.transform.position, Quaternion.identity, bullet.transform);

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(BulletNullifyRadius, 1f, BulletNullifyRadius);

        float timeElapsed = 0.0f;
        while (timeElapsed < TimeToExpandParryPassiveEffect)
        {
            if (spawnedNullification == null)
            {
                yield break;
            }
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

    void ApplyPassiveHardware_Yank(GameObject yankProjectile)
    {
        yankProjectile.GetComponent<Yank>().SetNullifying(this);
    }

    public void SpawnBulletNullification(GameObject bullet)
    {
        StartCoroutine(ApplyNullifyToBullet(bullet));
    }

    #endregion
}
