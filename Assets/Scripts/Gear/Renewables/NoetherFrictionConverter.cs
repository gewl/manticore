using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoetherFrictionConverter : EntityComponent, IRenewable {

    public RenewableTypes Type { get { return RenewableTypes.NoetherFrictionConverter; } }

    MobileEntityHealthComponent healthComponent;
    bool isActive = false;

    float healAmount = 20.0f;
    float cooldown = 8.0f;
    float duration = 4f;
    float cooldownCutFraction = 0.75f;

    float cooldownRemaining = 0.0f;
    float durationRemaining = 0.0f;

    float percentOfCooldownRemaining = 0.0f;
    float percentOfDurationRemaining = 0.0f;
    public CooldownDelegate CooldownPercentUpdater { get; set; }
    public CooldownDelegate CooldownDurationUpdater { get; set; }
    public DurationDelegate DurationUpdater { get; set; }

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
        if (cooldownRemaining >= 0.0f)
        {
            cooldownRemaining *= cooldownCutFraction;
        }
    }

    void OnUpdate()
    {
        if (cooldownRemaining > 0.0f)
        {
            cooldownRemaining -= Time.deltaTime;

            if (CooldownPercentUpdater != null)
            {
                float percentOfCooldownRemaining = cooldownRemaining / cooldown;
                CooldownPercentUpdater(percentOfCooldownRemaining);
                CooldownDurationUpdater(cooldownRemaining);
            }
        }
        if (isActive && durationRemaining > 0.0f)
        {
            durationRemaining -= Time.deltaTime;

            if (DurationUpdater != null)
            {
                float percentOfDurationRemaining = durationRemaining / duration;
                DurationUpdater(percentOfDurationRemaining);
            }
        }
        else if (isActive)
        {
            cooldownRemaining = cooldown;
            isActive = false;
        }
    }

    public void UseRenewable()
    {
        if (!isActive && cooldownRemaining <= 0.0f)
        {
            isActive = true;
            durationRemaining = duration;
        };
    }

}
