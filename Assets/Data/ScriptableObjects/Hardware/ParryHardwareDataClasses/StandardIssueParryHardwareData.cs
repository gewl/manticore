using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StandardIssueParryHardwareData : ParryHardwareData {

    protected override int baseStaminaCost
    {
        get { return 30; }
    }
    protected override int staminaCostMomentumIncrement
    {
        get { return 2; }
    }

    protected override float baseCooldown
    {
        get { return 0f; }
    }
    protected override float cooldownMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float baseMovementModifier
    {
        get { return 0.5f; }
    }
    protected override float movementModifierMomentumIncrement
    {
        get { return 0f; }
    }

    protected override float baseDamageDealt
    {
        get { return 50f; }
    }
    protected override float damageDealtMomentumIncrement
    {
        get { return 4f; }
    }

    protected override float baseTimeToCompleteParry
    {
        get { return 0.12f; }
    }
    protected override float timeToCompleteMomentumIncrement
    {
        get { return 0f; }
    }
}
