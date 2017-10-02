using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarComponent : EntityComponent {

    [SerializeField]
    Image healthBarContainer;
    [SerializeField]
    Image healthBarBackground;
    [SerializeField]
    Image healthBar;

    [SerializeField]
    Image damageBar;
    [SerializeField]
    float damageBarAdjustmentTime = 1f;
    [SerializeField]
    AnimationCurve damageBarAdjustmentCurve;

    [SerializeField]
    Color healthyColor;
    [SerializeField]
    Color warningColor;
    [SerializeField]
    Color dangerColor;

    float initialHealth;
    float barHeight = 15f;
    bool isAdjustingDamageBar = false;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.HealthChanged, OnHealthChanged);
        
        initialHealth = GameManager.GetPlayerInitialHealth();
        float barWidth = initialHealth;

        healthBarContainer.rectTransform.sizeDelta = new Vector2(barWidth + 4, barHeight + 4f);
        Vector2 startingBarSize = new Vector2(barWidth, barHeight);
        healthBarBackground.rectTransform.sizeDelta = startingBarSize;
        damageBar.rectTransform.sizeDelta = startingBarSize;
        healthBar.rectTransform.sizeDelta = startingBarSize;
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.HealthChanged, OnHealthChanged);
    }

    void OnHealthChanged()
    {
        float currentHealth = GameManager.GetPlayerCurrentHealth();
        float barWidth = currentHealth;

        healthBar.rectTransform.sizeDelta = new Vector2(barWidth, barHeight);
        float percentageOfHealthRemaining = currentHealth / initialHealth;
        if (percentageOfHealthRemaining <= 0.35f)
        {
            healthBar.color = dangerColor;
        }
        else if (percentageOfHealthRemaining <= 0.5f)
        {
            healthBar.color = warningColor;
        }
        else
        {
            healthBar.color = healthyColor;
        }
        if (isAdjustingDamageBar)
        {
            CancelInvoke();
            isAdjustingDamageBar = false;
        }
        StartCoroutine("DamageBarAdjustment");
    }

    IEnumerator DamageBarAdjustment()
    {
        isAdjustingDamageBar = true;

        Vector2 initialSize = damageBar.rectTransform.sizeDelta;
        Vector2 targetSize = healthBar.rectTransform.sizeDelta;
        float timeElapsed = 0f;

        while (timeElapsed < damageBarAdjustmentTime)
        {
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / damageBarAdjustmentTime;
            float curveCompletion = damageBarAdjustmentCurve.Evaluate(percentageComplete);

            Vector2 newDamageBarSize = new Vector2(Mathf.Lerp(initialSize.x, targetSize.x, curveCompletion), barHeight);
            damageBar.rectTransform.sizeDelta = newDamageBarSize;
            yield return null;
        }

        isAdjustingDamageBar = false;
        yield return null;
    }
     
}
