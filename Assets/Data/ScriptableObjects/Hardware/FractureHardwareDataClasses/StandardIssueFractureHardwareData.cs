using System;

[Serializable]
public class StandardIssueFractureHardwareData : FractureHardwareData {

    protected override int baseStaminaCost
    {
        get
        {
            return 25;
        }
    }

    protected override int staminaCostMomentumIncrement
    {
        get
        {
            return 2;
        }
    }

    protected override float baseCooldown
    {
        get
        {
            return 4f;
        }
    }

    protected override float cooldownMomentumIncrement
    {
        get
        {
            return 0.2f;
        }
    }

    protected override float BaseDamageDealt
    {
        get
        {
            return 30f;
        }
    }

    protected override float DamageDealtMomentumIncrement
    {
        get
        {
            return 4f;
        }
    }

    protected override float BaseFragmentationSpeedModifier
    {
        get
        {
            return 2f;
        }
    }

    protected override float FragmentationSpeedModifierMomentumIncrease
    {
        get
        {
            return 0.2f; 
        }
    }

    protected override float BaseRange
    {
        get
        {
            return 5f;
        }
    }

    protected override float RangeMomentumIncrement
    {
        get
        {
            return 1f;
        }
    }
}
