using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifierComponent : MonoBehaviour, IHardware {

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

    float nullifyRadius = 8f;
    float timeToComplete = 0.3f;

    void OnEnable()
    {
        nullifyEffect = (GameObject)Resources.Load(NULLIFY_PATH);
        nullifyMaterial = (Material)Resources.Load(NULLIFY_MATERIAL_PATH);
    }

    public void UseActiveHardware()
    {
        StartCoroutine("FireNullifyEffect");
    }

    public void ApplyPassiveHardware(HardwareTypes hardware, GameObject subject)
    {

    }

    IEnumerator FireNullifyEffect()
    {
        GameObject spawnedNullification = Instantiate(nullifyEffect, transform.position, Quaternion.identity);
        spawnedNullification.GetComponent<MeshRenderer>().material = nullifyMaterial;

        float timeElapsed = 0.0f;
        Vector3 originalSize = spawnedNullification.transform.localScale;
        Vector3 targetSize = new Vector3(nullifyRadius, 1f, nullifyRadius);

        while (timeElapsed < timeToComplete)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToComplete;
            float curveEval = GameManager.NullifyEffectCurve.Evaluate(percentageComplete);

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
}
