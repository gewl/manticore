using System;

[Serializable]
public abstract class FractureHardwareData : HardwareData {

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
    protected abstract float TravelTimeMomentumModifier { get; }

    public virtual float GetTravelTime(int currentMomentum)
    {
        return BaseTravelTime + (TravelTimeMomentumModifier * currentMomentum);
    }

    protected abstract float BaseLingerTime { get; }
    protected abstract float LingerTimeMomentumModifier { get; }

    public virtual float GetLingerTime(int currentMomentum)
    {
        return BaseLingerTime + (LingerTimeMomentumModifier * currentMomentum);
    }

    protected abstract int BaseNumberOfBullets { get; }
    protected abstract int NumberOfBulletsMomentumIncrease { get; }

    public virtual int GetNumberOfBullets(int currentMomentum)
    {
        return BaseNumberOfBullets + (NumberOfBulletsMomentumIncrease * currentMomentum);
    }

    protected abstract float BaseArcOfFire { get; }
    protected abstract float ArcOfFireMomentumModifier { get; }

    public virtual float GetArcOfFire(int currentMomentum)
    {
        return BaseArcOfFire + (ArcOfFireMomentumModifier * currentMomentum);
    }
}
