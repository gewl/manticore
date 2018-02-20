using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier {

    public enum ModifierType
    {
        MoveSpeed,
        DamageDealt,
        DamageReceived,
        Mark,
        Movement,
        Stun
    }

    ModifierType modifierType;
    public ModifierType GetModifierType { get { return modifierType; } }

    EntityModifierHandler modifierHandler;
    float baseDuration;
    float durationRemaining;

    public Modifier(EntityModifierHandler _modifierHandler)
    {
        modifierHandler = _modifierHandler;
        durationRemaining = baseDuration;
    }

    public void Update(float deltaTime)
    {
        durationRemaining -= deltaTime;

        if (durationRemaining <= 0.0f)
        {
            modifierHandler.DeregisterModifier(this);
        }
    }
}
