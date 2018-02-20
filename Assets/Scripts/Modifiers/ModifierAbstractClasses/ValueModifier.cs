using System;

[Serializable]
public abstract class ValueModifier : Modifier {

    public ValueModifier(EntityModifierHandler _modifierHandler) : base(_modifierHandler) { }

    protected abstract float ValueFactor { get; }

    public float ModifyValue(float currentValue)
    {
        return currentValue * ValueFactor;
    }
}
