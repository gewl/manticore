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
    Color healthyColor;
    [SerializeField]
    Color warningColor;
    [SerializeField]
    Color dangerColor;

    float initialHealth;

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.HealthChanged, OnHealthChanged);
        
        initialHealth = GameManager.GetPlayerInitialHealth();
        float barWidth = initialHealth;

        healthBarContainer.rectTransform.sizeDelta = new Vector2(barWidth + 4, 19f);
        healthBarBackground.rectTransform.sizeDelta = new Vector2(barWidth, 15f);
        healthBar.rectTransform.sizeDelta = new Vector2(barWidth, 15f);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.HealthChanged, OnHealthChanged);
    }

    void OnHealthChanged()
    {
        float currentHealth = GameManager.GetPlayerCurrentHealth();
        float barWidth = currentHealth;

        healthBar.rectTransform.sizeDelta = new Vector2(barWidth, 15f);
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
    }
}
