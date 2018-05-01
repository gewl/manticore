using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashController : MonoBehaviour {

    RawImage damageFlash;
    float flashTime = 0.2f;

    private void Start()
    {
        damageFlash = GetComponent<RawImage>();
    }

    public void FlashDamage()
    {
        StopAllCoroutines();

        StartCoroutine(BlinkImage());
    }

    IEnumerator BlinkImage()
    {
        float timeElapsed = 0.0f;
        Color flashColor = damageFlash.color;

        while (timeElapsed < flashTime)
        {
            float percentageComplete = timeElapsed / flashTime;

            flashColor.a = percentageComplete;

            damageFlash.color = flashColor;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        flashColor.a = 1f;
        damageFlash.color = flashColor;

        timeElapsed = 0.0f;

        while (timeElapsed < flashTime)
        {
            float percentageComplete = 1 - (timeElapsed / flashTime);

            flashColor.a = percentageComplete;

            damageFlash.color = flashColor;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        flashColor.a = 0.0f;
        damageFlash.color = flashColor;
    }
}
