using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumManagerComponent : EntityComponent {

    Dictionary<HardwareTypes, int> hardwareTypeToMomentumMap;
    // This tracks in what order & to which Hardware momentum was assigned.
    // When player loses momentum, the last assigned momentum is removed.
    Stack<HardwareTypes> assignedMomentumTracker;

    int unassignedAvailableMomentum = 0;

    protected override void Awake()
    {
        base.Awake();

        hardwareTypeToMomentumMap = new Dictionary<HardwareTypes, int>();
    }

    protected override void Subscribe()
    {
        InventoryController.OnInventoryUpdated += ClearMomentum;
    }

    protected override void Unsubscribe()
    {
    }

    public int GetMomentumByHardwareType(HardwareTypes hardwareType)
    {
        if (!hardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            hardwareTypeToMomentumMap[hardwareType] = 0;
        }

        return hardwareTypeToMomentumMap[hardwareType];
    }

    void IncreaseMomentumForHardware(HardwareTypes hardwareType)
    {
        if (!hardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            hardwareTypeToMomentumMap[hardwareType] = 0;
        }

        hardwareTypeToMomentumMap[hardwareType]++;
        assignedMomentumTracker.Push(hardwareType);
    }

    void UndoLastMomentumIncrease()
    {
        if (assignedMomentumTracker.Count <= 0f)
        {
            Debug.Log("Trying to undo last momentum assignment with no momentum assigned.");
            return;
        }
        HardwareTypes lastHardwareTypeIncremented = assignedMomentumTracker.Pop();

        hardwareTypeToMomentumMap[lastHardwareTypeIncremented]--;
    }

    void ClearMomentum(InventoryData inventory)
    {
        hardwareTypeToMomentumMap.Clear();
        assignedMomentumTracker.Clear();
    }

}
