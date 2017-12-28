using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override float baseNullifyRadius
    {
        get { return 8f; }
    }
    protected override float nullifyRadiusMomentumIncrement
    {
        get { return 0.4f; }
    }

    protected override float baseLingerDuration
    {
        get { return 0.2f; }
    }
    protected override float lingerDurationMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float baseTimeToComplete
    {
        get { return 0.3f; }
    }
    protected override float timeToCompleteMomentumIncrement
    {
        get { return 0.02f; }
    }

}
