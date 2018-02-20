using System;

[Serializable]
public abstract class NullifyHardwareData : HardwareData {

    protected abstract float BaseNullifyRadius { get; }
    protected abstract float NullifyRadiusMomentumIncrement { get; }

    public virtual float GetNullifyRadius(int currentMomentum)
    {
        return BaseNullifyRadius + (NullifyRadiusMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseTimeToExpand { get; }
    protected abstract float TimeToExpandMomentumDecrement { get; }

    public virtual float GetTimeToExpand(int currentMomentum)
    {
        return BaseTimeToExpand - (TimeToExpandMomentumDecrement * currentMomentum);
    }

    protected abstract float BaseLingerDuration { get; }
    protected abstract float LingerDurationMomentumIncrement { get; }

    public virtual float GetLingerDuration(int currentMomentum)
    {
        return BaseLingerDuration + (LingerDurationMomentumIncrement * currentMomentum);
    }
}
