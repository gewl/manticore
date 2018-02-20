using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveSpeedModifier : Modifier {

    protected virtual float moveSpeedFactor { get; set; }

    public MoveSpeedModifier(EntityModifierHandler _modifierHandler) : base(_modifierHandler) { }

    public float ModifyMoveSpeed(float currentMoveSpeed)
    {
        return currentMoveSpeed * moveSpeedFactor;
    }

}
