﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDStaminaBar : StaminaBar {

    Image staminaBarContainer;
    [SerializeField]
    Image staminaBar;

    [SerializeField]
    Image damageBar;
    [SerializeField]
    float damageBarAdjustmentTime = 1f;
    [SerializeField]
    AnimationCurve damageBarAdjustmentCurve;

    EntityStaminaComponent manticoreStaminaComponent;
    float totalStamina;
    float barHeight;
    float barWidth = 0f;
    float barContainerHeight;
    bool isAdjustingDamageBar = false;

    private void Awake()
    {
        staminaBarContainer = GetComponent<Image>();
        manticoreStaminaComponent = GameManager.GetPlayerTransform().GetComponent<EntityStaminaComponent>();
        barHeight = staminaBar.rectTransform.sizeDelta.y;

        barContainerHeight = staminaBarContainer.rectTransform.rect.height;
    }

    private void OnEnable()
    {
        manticoreStaminaComponent.TotalStaminaUpdated += UpdateTotalStamina;
        manticoreStaminaComponent.CurrentStaminaUpdated += UpdateCurrentStamina;
    }

    private void OnDisable()
    {
        manticoreStaminaComponent.TotalStaminaUpdated -= UpdateTotalStamina;
        manticoreStaminaComponent.CurrentStaminaUpdated -= UpdateCurrentStamina;
    }

    public override void UpdateTotalStamina(float newTotalStamina)
    {
        totalStamina = newTotalStamina;

        barWidth = totalStamina * 2f;

        staminaBarContainer.rectTransform.sizeDelta = new Vector2(barWidth + 30f, barContainerHeight);
        Vector2 startingBarSize = new Vector2(barWidth, barHeight);
        damageBar.rectTransform.sizeDelta = startingBarSize;
        staminaBar.rectTransform.sizeDelta = startingBarSize;
    }

    public override void UpdateCurrentStamina(float newCurrentStamina)
    {
        float newBarWidth = newCurrentStamina * 2f;

        staminaBar.rectTransform.sizeDelta = new Vector2(newBarWidth, barHeight);

        if (newBarWidth < barWidth)
        {
            if (isAdjustingDamageBar)
            {
                CancelInvoke();
                isAdjustingDamageBar = false;
            }
            StartCoroutine("DamageBarAdjustment");
        }
        else
        {
            damageBar.rectTransform.sizeDelta = new Vector2(newBarWidth, barHeight);
        }

        barWidth = newBarWidth;
    }

    IEnumerator DamageBarAdjustment()
    {
        isAdjustingDamageBar = true;

        Vector2 initialSize = damageBar.rectTransform.sizeDelta;
        Vector2 targetSize = staminaBar.rectTransform.sizeDelta;
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
