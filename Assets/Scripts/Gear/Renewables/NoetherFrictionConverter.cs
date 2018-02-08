using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoetherFrictionConverter : EntityComponent, IRenewable {

    MobileEntityHealthComponent healthComponent;
    bool isActive = false;

    float healAmount = 20.0f;
    float cooldown = 12.0f;
    float duration = 4f;
    float cooldownCutFraction = 0.75f;

    float currentCooldown = 0.0f;
    float durationRemaining = 0.0f;

    protected override void Subscribe()
    {
        healthComponent = GetComponent<MobileEntityHealthComponent>();
        entityEmitter.SubscribeToEvent(EntityEvents.Parry, OnParry);
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Parry, OnParry);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnParry()
    {
        if (isActive)
        {
            healthComponent.GetHealed(healAmount);
        }
        if (currentCooldown >= 0.0f)
        {
            currentCooldown *= cooldownCutFraction;
        }
    }

    void OnUpdate()
    {
        if (currentCooldown > 0.0f)
        {
            currentCooldown -= Time.deltaTime;
        }
        if (isActive && durationRemaining > 0.0f)
        {
            durationRemaining -= Time.deltaTime;
        }
        else if (isActive)
        {
            isActive = false;
        }
    }

    public void UseRenewable()
    {
        if (!isActive && currentCooldown <= 0.0f)
        {
            isActive = true;
            currentCooldown = cooldown;
            durationRemaining = duration;
        };
    }

}
