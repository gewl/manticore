using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlinkHardwareData : HardwareData {

    protected abstract float baseBlinkRange { get; }
    protected abstract float blinkRangeMomentumIncrement { get; }

    public virtual float GetBlinkRange(int currentMomentum)
    {
        return baseBlinkRange + (blinkRangeMomentumIncrement * currentMomentum);
    }

    protected abstract float baseTimeToCompleteBlink { get; }
    protected abstract float timeToCompleteMomentumIncrement { get; }

    public virtual float GetTimeToCompleteBlink(int currentMomentum)
    {
        return baseTimeToCompleteBlink + (timeToCompleteMomentumIncrement * currentMomentum);
    }
}
