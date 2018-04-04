﻿using System;

[Serializable]
public abstract class YankHardwareData : HardwareData {
    protected abstract float baseDamageDealt { get; }
    protected abstract float damageDealtMomentumIncrement { get; }

    public virtual float GetDamageDealt(int currentMomentum)
    {
        return baseDamageDealt + (damageDealtMomentumIncrement * currentMomentum);
    }

    protected abstract float baseRange { get; }
    protected abstract float rangeMomentumIncrement { get; }

    public virtual float GetRange(int currentMomentum)
    {
        return baseRange + (rangeMomentumIncrement * currentMomentum);
    }

    protected abstract float baseTravelTime { get; }
    protected abstract float travelTimeMomentumIncrement { get; }

    public virtual float GetTravelTime(int currentMomentum)
    {
        return baseTravelTime + (travelTimeMomentumIncrement * currentMomentum);
    }

}
