using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RiposteHardwareData : HardwareData {

    protected abstract float baseDamageDealt { get; }
    protected abstract float damageDealtMomentumIncrement { get; }

    public virtual float GetDamageDealt(int currentMomentum)
    {
        return baseDamageDealt + (damageDealtMomentumIncrement * currentMomentum);
    }

    protected abstract float baseDashRange { get; }
    protected abstract float dashRangeMomentumIncrement { get; }

    public virtual float GetDashRange(int currentMomentum)
    {
        return baseDashRange + (dashRangeMomentumIncrement * currentMomentum);
    }

    protected abstract float baseDashRangeIncreaseRate { get; }
    protected abstract float dashRangeIncreaseRateMomentumIncrement { get; }

    public virtual float GetDashRangeIncreaseRate(int currentMomentum)
    {
        return baseDashRangeIncreaseRate + (dashRangeIncreaseRateMomentumIncrement * currentMomentum);
    }
}
