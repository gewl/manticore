using System;

[Serializable]
public abstract class BlinkHardwareData : HardwareData {

    protected abstract float BaseBlinkRange { get; }
    protected abstract float BlinkRangeMomentumIncrement { get; }

    public virtual float GetBlinkRange(int currentMomentum)
    {
        return BaseBlinkRange + (BlinkRangeMomentumIncrement * currentMomentum);
    }

    protected abstract float BaseTimeToCompleteBlink { get; }
    protected abstract float TimeToCompleteMomentumIncrement { get; }

    public virtual float GetTimeToCompleteBlink(int currentMomentum)
    {
        return BaseTimeToCompleteBlink + (TimeToCompleteMomentumIncrement * currentMomentum);
    }
}
