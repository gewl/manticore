using System;
using UnityEngine;

[CreateAssetMenu(menuName="Modifiers/ValueModifier")]
public class ValueModifier : Modifier {

    public float valueFactor;

    public float ModifyValue(float currentValue)
    {
        return currentValue * valueFactor;
    }
}
