using System;
using UnityEngine;

[Serializable]
public abstract class HardwareData : ScriptableObject {

    protected abstract int baseStaminaCost { get; }
    protected abstract int staminaCostMomentumIncrement { get; }

    public virtual int GetStaminaCost(int currentMomentum)
    {
        return baseStaminaCost - (staminaCostMomentumIncrement * currentMomentum);
    }

    protected abstract float baseCooldown { get; }
    protected abstract float cooldownMomentumIncrement { get; }

    public virtual float GetCooldown(int currentMomentum)
    {
        return baseCooldown - (cooldownMomentumIncrement * currentMomentum);
    }

}
