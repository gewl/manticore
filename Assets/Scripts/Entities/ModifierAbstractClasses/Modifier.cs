﻿using System.Collections;
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

    public virtual ModifierType GetModifierType { get; set; }
    public virtual float BaseDuration { get; set; }

    EntityModifierHandler modifierHandler;
    float durationRemaining;

    public Modifier(EntityModifierHandler _modifierHandler)
    {
        modifierHandler = _modifierHandler;
        durationRemaining = BaseDuration;
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