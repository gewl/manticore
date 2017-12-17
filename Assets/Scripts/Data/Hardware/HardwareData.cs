using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HardwareData : ScriptableObject {

    protected abstract int baseStaminaCost { get; }
    protected abstract int staminaCostMomentumIncrement { get; }

    public virtual int GetStaminaCost(int currentMomentum)
    {
        return baseStaminaCost - (staminaCostMomentumIncrement * currentMomentum);
    }

    protected abstract int baseCooldown { get; }
    protected abstract int cooldownMomentumIncrement { get; }

    public virtual int GetCooldown(int currentMomentum)
    {
        return baseCooldown - (cooldownMomentumIncrement * currentMomentum);
    }

}
