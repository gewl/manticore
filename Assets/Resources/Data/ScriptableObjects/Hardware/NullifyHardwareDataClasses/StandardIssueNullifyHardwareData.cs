using System;

[Serializable]
public class StandardIssueNullifyHardwareData : NullifyHardwareData {

    protected override int baseStaminaCost
    {
        get { return 80; }
    }
    protected override int staminaCostMomentumIncrement
    {
        get { return 4; }
    }

    protected override float baseCooldown
    {
        get { return 4f; }
    }
    protected override float cooldownMomentumIncrement
    {
        get { return 0.25f; }
    }

    protected override float BaseNullifyRadius
    {
        get { return 8f; }
    }
    protected override float NullifyRadiusMomentumIncrement
    {
        get { return 0.4f; }
    }

    protected override float BaseLingerDuration
    {
        get { return 0.8f; }
    }
    protected override float LingerDurationMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float BaseTimeToExpand
    {
        get { return 0.3f; }
    }
    protected override float TimeToExpandMomentumDecrement
    {
        get { return 0.02f; }
    }

}
