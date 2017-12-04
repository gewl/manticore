using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifierHardware : MonoBehaviour, IHardware {

    HardwareTypes type = HardwareTypes.Nullify;
    public HardwareTypes Type { get { return type; } }

    public bool IsInUse { get { return false; } }

    HardwareUseTypes hardwareUseType = HardwareUseTypes.Instant;
    public HardwareUseTypes HardwareUseType { get { return hardwareUseType; } }

    private int baseStaminaCost = 40;
    public int BaseStaminaCost
    {
        get
        {
            return baseStaminaCost;
        }
    }

    private int updatedStaminaCost = 40;
    public int UpdatedStaminaCost
    {
        get
        {
            return updatedStaminaCost;
        }
    }

    private bool isOnCooldown = false;
    public bool IsOnCooldown
    {
        get
        {
            return isOnCooldown;
        }
    }

    GameObject nullifyEffect;
    const string NULLIFY_PATH = "Prefabs/Abilities/NullifierEffect";
    Material nullifyMaterial;
    const string NULLIFY_MATERIAL_PATH = "Materials/CharacterParts/Effects/NullifySkin";

    float cooldownTime = 2f;

    // Active hardware values
    float entityNullifyRadius = 8f;
    float timeToCompleteActiveEffect = 0.3f;

    // Passive hardware (Parry) values
    float bulletNullifyRadius = 4f;
    float timeToExpandPassiveEffect = 0.1f;

    void OnEnable()
    {
        nullifyEffect = (GameObject)Resources.Load(NULLIFY_PATH);
        nullifyMaterial = (Material)Resources.Load(NULLIFY_MATERIAL_PATH);
    }

    #region Active hardware use
    public void UseActiveHardware()
    {
        StartCoroutine(FireNullifyEffect(timeToCompleteActiveEffect));
    }

    IEnumerator FireNullifyEffect(float duration, bool shouldFollow = false)
    {
        GameObject spawnedNullification = Instantiate(nullifyEffect, transform.position, Quaternion.identity);
        spawnedNullification.GetComponent<MeshRenderer>().material = nullifyMaterial;

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

        DestroyObject(spawnedNullification);
        yield break;
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownTime);

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
                break;
            case HardwareTypes.Riposte:
                break;
            default:
                break;
        }
    }

    IEnumerator ApplyPassiveHardware_Parry(GameObject bullet)
    {
        GameObject spawnedNullification = Instantiate(nullifyEffect, bullet.transform.position, Quaternion.identity, bullet.transform);
        spawnedNullification.GetComponent<MeshRenderer>().material = nullifyMaterial;

        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(bulletNullifyRadius, 1f, bulletNullifyRadius);

        float timeElapsed = 0.0f;
        while (timeElapsed < timeToExpandPassiveEffect)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToCompleteActiveEffect;
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

        StartCoroutine(FireNullifyEffect(blinkDuration, true));

        yield return null;
    }
    #endregion
}
