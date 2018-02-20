using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueModifier : Modifier {

    public ValueModifier(EntityModifierHandler _modifierHandler) : base(_modifierHandler) { }

    protected virtual float ValueFactor { get; set; }

    public float ModifyValue(float currentValue)
    {
        return currentValue * ValueFactor;
    }
}
