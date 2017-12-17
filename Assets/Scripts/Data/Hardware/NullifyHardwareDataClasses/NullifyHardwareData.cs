using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NullifyHardwareData : HardwareData {

    protected abstract float baseNullifyRadius { get; }
    protected abstract float nullifyRadiusMomentumIncrement { get; }

    public virtual float GetNullifyRadius(int currentMomentum)
    {
        return baseNullifyRadius + (nullifyRadiusMomentumIncrement * currentMomentum);
    }

    protected abstract float baseTimeToComplete { get; }
    protected abstract float timeToCompleteMomentumIncrement { get; }

    public virtual float GetTimeToComplete(int currentMomentum)
    {
        return baseTimeToComplete + (timeToCompleteMomentumIncrement * currentMomentum);
    }

    protected abstract float baseLingerDuration { get; }
    protected abstract float lingerDurationMomentumIncrement { get; }

    public virtual float GetLingerDuration(int currentMomentum)
    {
        return baseLingerDuration + (lingerDurationMomentumIncrement * currentMomentum);
    }
}
