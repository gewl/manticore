using System;

[Serializable]
public abstract class YankHardwareData : HardwareData {
    protected abstract float BaseDamageDealt { get; }
    protected abstract float DamageDealtMomentumIncrement { get; }

    public virtual float GetDamageDealt(int currentMomentum)
    {
        return BaseDamageDealt + (DamageDealtMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseRange { get; }
    protected abstract float RangeMomentumIncrement { get; }

    public virtual float GetRange(int currentMomentum)
    {
        return BaseRange + (RangeMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseTravelTime { get; }
    protected abstract float TravelTimeMomentumIncrement { get; }

    public virtual float GetTravelTime(int currentMomentum)
    {
        return BaseTravelTime + (TravelTimeMomentumIncrement * currentMomentum);
    }

}
