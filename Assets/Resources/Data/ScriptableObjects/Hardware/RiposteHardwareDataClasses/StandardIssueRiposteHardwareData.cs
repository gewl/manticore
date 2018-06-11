using System;

[Serializable]
public class StandardIssueRiposteHardwareData : RiposteHardwareData {

    protected override int baseStaminaCost
    {
        get { return 10; }
    }
    protected override int staminaCostMomentumIncrement
    {
        get { return 0; }
    }

    protected override float baseCooldown
    {
        get { return 4f; }
    }
    protected override float cooldownMomentumIncrement
    {
        get { return 0.2f; }
    }

    protected override float baseDamageDealt
    {
        get { return 100f; }
    }
    protected override float damageDealtMomentumIncrement
    {
        get { return 4f; }
    }

    protected override float baseDashRange
    {
        get { return 10f; }
    }
    protected override float dashRangeMomentumIncrement
    {
        get { return 1f; }
    }

    protected override float baseDashRangeIncreaseRate
    {
        get { return 8f; }
    }
    protected override float dashRangeIncreaseRateMomentumIncrement
    {
        get { return 0.4f; }
    }
}
