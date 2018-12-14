using System;

[Serializable]
public class StandardIssueBlinkHardwareData : BlinkHardwareData {

    protected override int baseStaminaCost
    {
        get { return 60; }
    }
    protected override int staminaCostMomentumIncrement
    {
        get { return 5; }
    }

    protected override float baseCooldown
    {
        get { return 2.0f; }
    }
    protected override float cooldownMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float BaseBlinkRange
    {
        get { return 15f; }
    }
    protected override float BlinkRangeMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float BaseTimeToCompleteBlink
    {
        get { return 0.2f; }
    }
    protected override float TimeToCompleteMomentumIncrement
    {
        get { return 0f; }
    }
}
