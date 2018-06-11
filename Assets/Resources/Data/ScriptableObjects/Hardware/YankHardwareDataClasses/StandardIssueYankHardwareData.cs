using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardIssueYankHardwareData : YankHardwareData {

    protected override int baseStaminaCost
    {
        get
        {
            return 15;
        }
    }

    protected override int staminaCostMomentumIncrement
    {
        get
        {
            return 1;
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
            return 0.4f;
        }
    }

    protected override float BaseDamageDealt
    {
        get
        {
            return 50f;
        }
    }

    protected override float DamageDealtMomentumIncrement
    {
        get
        {
            return 4f;
        }
    }

    protected override float BaseRange
    {
        get
        {
            return 15f;
        }
    }

    protected override float RangeMomentumIncrement
    {
        get
        {
            return 2f;
        }
    }

    protected override float BaseTravelTime
    {
        get
        {
            return 0.8f;
        }
    }

    protected override float TravelTimeMomentumIncrement
    {
        get
        {
            return 0.08f;
        }
    }
}
