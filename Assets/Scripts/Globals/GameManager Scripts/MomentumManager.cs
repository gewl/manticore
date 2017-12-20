using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumManager : MonoBehaviour {

    static Dictionary<HardwareTypes, int> hardwareTypeToMomentumMap;
    // This tracks in what order & to which Hardware momentum was assigned.
    // When player loses momentum, the last assigned momentum is removed.
    // Count also used to track total assigned momentum.
    static Stack<HardwareTypes> assignedMomentumTracker;

    int progressTowardNextMomentum = 0;
    int momentumRequiredForNextPoint
    {
        get
        {
            return (assignedMomentumTracker.Count + 1) * 5;
        }
    }

    int unassignedAvailableMomentumPoints = 0;

    protected void Awake()
    {
        assignedMomentumTracker = new Stack<HardwareTypes>();
        hardwareTypeToMomentumMap = new Dictionary<HardwareTypes, int>();
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += ClearMomentum;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= ClearMomentum;
    }

    #region momentum event handlers

    static void HandleEntityDeathEvent(GlobalConstants.EntityTypes entityType)
    {
        
    }

    #endregion

    #region point manipulation
    public static int GetMomentumPointsByHardwareType(HardwareTypes hardwareType)
    {
        if (!hardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            hardwareTypeToMomentumMap[hardwareType] = 0;
        }

        return hardwareTypeToMomentumMap[hardwareType];
    }

    static void AssignMomentumPointToHardware(HardwareTypes hardwareType)
    {
        if (!hardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            hardwareTypeToMomentumMap[hardwareType] = 0;
        }

        hardwareTypeToMomentumMap[hardwareType]++;
        assignedMomentumTracker.Push(hardwareType);
    }

    static void RemoveLastMomentumPoint()
    {
        if (assignedMomentumTracker.Count <= 0f)
        {
            Debug.Log("Trying to undo last momentum assignment with no momentum assigned.");
            return;
        }
        HardwareTypes lastHardwareTypeIncremented = assignedMomentumTracker.Pop();

        hardwareTypeToMomentumMap[lastHardwareTypeIncremented]--;
    }

    static void ClearMomentum(InventoryData inventory)
    {
        hardwareTypeToMomentumMap.Clear();
        assignedMomentumTracker.Clear();
    }
    #endregion
}
