using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModifierHandler : EntityComponent {

    List<ValueModifier> activeMoveSpeedModifiers;
    List<ValueModifier> activeDamageDealtModifiers;
    List<ValueModifier> activeDamageReceivedModifiers;

    Modifier activeStunModifier;

    protected override void Awake()
    {
        base.Awake();
        activeMoveSpeedModifiers = new List<ValueModifier>();
        activeDamageDealtModifiers = new List<ValueModifier>();
        activeDamageReceivedModifiers = new List<ValueModifier>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }

    void OnUpdate()
    {
        float timeElapsed = Time.deltaTime;

        for (int i = 0; i < activeMoveSpeedModifiers.Count; i++)
        {
            activeMoveSpeedModifiers[i].UpdateModifierDuration(timeElapsed);
        }
        for (int i = 0; i < activeDamageDealtModifiers.Count; i++)
        {
            activeDamageDealtModifiers[i].UpdateModifierDuration(timeElapsed);
        }
        for (int i = 0; i < activeDamageReceivedModifiers.Count; i++)
        {
            activeDamageReceivedModifiers[i].UpdateModifierDuration(timeElapsed);
        }
    }

    public void RegisterModifier(Modifier newModifier)
    {
        newModifier.Init(this);
        switch (newModifier.modifierType)
        {
            case ModifierType.MoveSpeed:
                activeMoveSpeedModifiers.Add(newModifier as ValueModifier);
                break;
            case ModifierType.DamageDealt:
                activeDamageDealtModifiers.Add(newModifier as ValueModifier);
                break;
            case ModifierType.DamageReceived:
                activeDamageReceivedModifiers.Add(newModifier as ValueModifier);
                break;
            case ModifierType.Mark:
                break;
            case ModifierType.Movement:
                break;
            case ModifierType.Stun:
                if (activeStunModifier == null)
                {
                    activeStunModifier = newModifier;
                }
                else if (newModifier.baseDuration > activeStunModifier.DurationRemaining)
                {
                    Destroy(activeStunModifier);
                    activeStunModifier = newModifier;
                }
                else
                {
                    Destroy(newModifier);
                }
                entityEmitter.isStunned = true;
                break;
            default:
                break;
        }
    }

    public void DeregisterModifier(Modifier modifierToDeregister)
    {
        switch (modifierToDeregister.modifierType)
        {
            case ModifierType.MoveSpeed:
                activeMoveSpeedModifiers.Remove(modifierToDeregister as ValueModifier);
                break;
            case ModifierType.DamageDealt:
                activeDamageDealtModifiers.Remove(modifierToDeregister as ValueModifier);
                break;
            case ModifierType.DamageReceived:
                activeDamageReceivedModifiers.Remove(modifierToDeregister as ValueModifier);
                break;
            case ModifierType.Mark:
                break;
            case ModifierType.Movement:
                break;
            case ModifierType.Stun:
                if (modifierToDeregister == activeStunModifier)
                {
                    Destroy(modifierToDeregister);
                    activeStunModifier = null;
                    entityEmitter.isStunned = false;
                }
                break;
            default:
                break;
        }
    }

    List<ValueModifier> GetValueModifierList(ModifierType modifierType)
    {
        switch (modifierType)
        {
            case ModifierType.MoveSpeed:
                return activeMoveSpeedModifiers;
            case ModifierType.DamageDealt:
                return activeDamageDealtModifiers;
            case ModifierType.DamageReceived:
                return activeDamageReceivedModifiers;
            default:
                Debug.LogError("Trying to get ValueModifierList for incompatible ModifierType: " + modifierType);
                return null;
        }
    }

    public float ApplyModifiersToValue(ModifierType modifierType, float currentValue)
    {
        List<ValueModifier> activeModifierList = GetValueModifierList(modifierType);

        int numberOfActiveModifiers = activeModifierList.Count;

        if (numberOfActiveModifiers == 0)
        {
            return currentValue;
        }

        for (int i = 0; i < numberOfActiveModifiers; i++)
        {
            currentValue = activeMoveSpeedModifiers[i].ModifyValue(currentValue);
        }

        return currentValue;
    }
}
