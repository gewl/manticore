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
            return 20f;
        }
    }

    protected override float DamageDealtMomentumIncrement
    {
        get
        {
            return 4f;
        }
    }

    protected override float BaseTravelTime
    {
        get
        {
            return 0.5f;
        }
    }

    protected override float TravelTimeMomentumModifier
    {
        get
        {
            return .02f; 
        }
    }

    protected override float BaseLingerTime
    {
        get
        {
            return 0.2f;
        }
    }

    protected override float LingerTimeMomentumModifier
    {
        get
        {
            return .04f; 
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

    protected override int BaseNumberOfBullets
    {
        get
        {
            return 4;
        }
    }

    protected override int NumberOfBulletsMomentumIncrease
    {
        get
        {
            return 1;
        }
    }

    protected override float BaseArcOfFire
    {
        get
        {
            return 10f;
        }
    }

    protected override float ArcOfFireMomentumModifier
    {
        get
        {
            return -2f;
        }
    }
}
