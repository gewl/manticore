using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardIssueBlinkHardwareData : BlinkHardwareData {

    protected override int baseStaminaCost
    {
        get { return 40; }
    }
    protected override int staminaCostMomentumIncrement
    {
        get { return 7; }
    }

    protected override float baseCooldown
    {
        get { return 1.0f; }
    }
    protected override float cooldownMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float baseBlinkRange
    {
        get { return 15f; }
    }
    protected override float blinkRangeMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float baseTimeToCompleteBlink
    {
        get { return 0.2f; }
    }
    protected override float timeToCompleteMomentumIncrement
    {
        get { return 0f; }
    }
}
