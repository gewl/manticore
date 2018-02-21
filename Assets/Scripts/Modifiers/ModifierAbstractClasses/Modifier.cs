using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : ScriptableObject {

    public ModifierType modifierType;
    public float baseDuration;

    EntityModifierHandler modifierHandler;
    float durationRemaining;

    public void Init(EntityModifierHandler _modifierHandler)
    {
        modifierHandler = _modifierHandler;
        durationRemaining = baseDuration;
    }

    public void UpdateModifierDuration(float deltaTime)
    {
        durationRemaining -= deltaTime;

        if (durationRemaining <= 0.0f)
        {
            modifierHandler.DeregisterModifier(this);
        }
    }
}
