using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifierComponent : EntityComponent {

    [SerializeField]
    GameObject nullifyEffect;

    [SerializeField]
    float nullifyRadius = 5f;
    [SerializeField]
    float timeToComplete = 0.2f;
    [SerializeField]
    Material nullifyMaterial;
    [SerializeField]
    AnimationCurve nullifyCurve;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Nullify, OnNullify);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Nullify, OnNullify);
    }
    
    void OnNullify()
    {
        StartCoroutine("FireNullifyEffect");
    }

    IEnumerator FireNullifyEffect()
    {
        GameObject activeNullifyEffect = Instantiate(nullifyEffect, transform.position, Quaternion.identity);
        activeNullifyEffect.GetComponent<MeshRenderer>().material = nullifyMaterial;

        float timeElapsed = 0.0f;
        Vector3 originalSize = activeNullifyEffect.transform.localScale;
        Vector3 targetSize = new Vector3(nullifyRadius, 1f, nullifyRadius);

        while (timeElapsed < timeToComplete)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToComplete;
            float curveEval = nullifyCurve.Evaluate(percentageComplete);

            activeNullifyEffect.transform.localScale = Vector3.Lerp(originalSize, targetSize, curveEval);
            yield return null;
        }

        DestroyObject(activeNullifyEffect);
        yield break;
    }
}
