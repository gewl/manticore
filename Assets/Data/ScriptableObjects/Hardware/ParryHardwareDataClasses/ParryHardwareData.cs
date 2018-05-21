using System;

[Serializable]
public abstract class ParryHardwareData : HardwareData {

    protected abstract float baseDamageDealt { get; }
    protected abstract float damageDealtMomentumIncrement { get; }

    public virtual float GetDamageDealt(int currentMomentum)
    {
        return baseDamageDealt + (damageDealtMomentumIncrement * currentMomentum);
    }

    protected abstract float baseMovementModifier { get; }
    protected abstract float movementModifierMomentumIncrement { get; }

    public virtual float GetMovementModifier(int currentMomentum)
    {
        return baseMovementModifier - (movementModifierMomentumIncrement * currentMomentum);
    }

    protected virtual float baseTimeToCombo { get { return 0.3f; } }
    protected virtual float timeToComboMomentumIncrement { get { return 0.05f; } }

    public virtual float GetTimeToCombo(int currentMomentum)
    {
        return baseTimeToCombo + (timeToComboMomentumIncrement * currentMomentum);
    }
}
