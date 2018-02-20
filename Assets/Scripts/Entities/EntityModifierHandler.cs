using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModifierHandler : MonoBehaviour {

    List<MoveSpeedModifier> activeMoveSpeedModifiers;

    private void Awake()
    {
        activeMoveSpeedModifiers = new List<MoveSpeedModifier>();
    }

    public void RegisterModifier(Modifier modifier)
    {

    }

    public void DeregisterModifier(Modifier modifier)
    {

    }

    float ApplyModifiersToMoveSpeed(float currentMoveSpeed)
    {
        int numberOfActiveModifiers = activeMoveSpeedModifiers.Count;

        if (numberOfActiveModifiers == 0)
        {
            return currentMoveSpeed;
        }

        for (int i = 0; i < numberOfActiveModifiers; i++)
        {
            currentMoveSpeed = activeMoveSpeedModifiers[i].ModifyMoveSpeed(currentMoveSpeed);
        }

        return currentMoveSpeed;
    }
}
