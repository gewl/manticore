using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModifierHandler : EntityComponent {

    List<ValueModifier> activeMoveSpeedModifiers;
    List<ValueModifier> activeDamageDealtModifiers;
    List<ValueModifier> activeDamageReceivedModifiers;

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
            activeMoveSpeedModifiers[i].Update(timeElapsed);
        }
    }

    public void RegisterModifier(Modifier modifier)
    {
        switch (modifier.GetModifierType)
        {
            case Modifier.ModifierType.MoveSpeed:
                activeMoveSpeedModifiers.Add(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.DamageDealt:
                activeDamageDealtModifiers.Add(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.DamageReceived:
                activeDamageReceivedModifiers.Add(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.Mark:
                break;
            case Modifier.ModifierType.Movement:
                break;
            case Modifier.ModifierType.Stun:
                break;
            default:
                break;
        }
    }

    public void DeregisterModifier(Modifier modifier)
    {
        switch (modifier.GetModifierType)
        {
            case Modifier.ModifierType.MoveSpeed:
                activeMoveSpeedModifiers.Remove(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.DamageDealt:
                activeDamageDealtModifiers.Remove(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.DamageReceived:
                activeDamageReceivedModifiers.Remove(modifier as ValueModifier);
                break;
            case Modifier.ModifierType.Mark:
                break;
            case Modifier.ModifierType.Movement:
                break;
            case Modifier.ModifierType.Stun:
                break;
            default:
                break;
        }
    }

    List<ValueModifier> GetValueModifierList(Modifier.ModifierType modifierType)
    {
        switch (modifierType)
        {
            case Modifier.ModifierType.MoveSpeed:
                return activeMoveSpeedModifiers;
            case Modifier.ModifierType.DamageDealt:
                return activeDamageDealtModifiers;
            case Modifier.ModifierType.DamageReceived:
                return activeDamageReceivedModifiers;
            default:
                Debug.LogError("Trying to get ValueModifierList for incompatible ModifierType: " + modifierType);
                return null;
        }
    }

    public float ApplyModifiersToValue(Modifier.ModifierType modifierType, float currentValue)
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
