using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumManager : MonoBehaviour {

    public delegate void MomentumPointsUpdatedDelegate(int availablePoints);
    public static MomentumPointsUpdatedDelegate OnAvailableMomentumPointsUpdated;

    public delegate void AssignedMomentumPointsUpdatedDelegate(Dictionary<HardwareTypes, int> hardwareTypeToMomentumMap);
    public static AssignedMomentumPointsUpdatedDelegate OnAssignedMomentumPointsUpdated;

    static Dictionary<HardwareTypes, int> hardwareTypeToMomentumMap;
    // This tracks in what order & to which Hardware momentum was assigned.
    // When player loses momentum, the last assigned momentum is removed.
    // Count also used to track total assigned momentum.
    static Stack<HardwareTypes> assignedMomentumTracker;

    static int progressTowardNextMomentum = 0;
    static int momentumRequiredForNextPoint
    {
        get
        {
            return (assignedMomentumTracker.Count  + unassignedAvailableMomentumPoints + 1) * 5;
        }
    }

    static int unassignedAvailableMomentumPoints = 0;
    public static int UnassignedAvailableMomentumPoints { get { return unassignedAvailableMomentumPoints; } }

    protected void Awake()
    {
        assignedMomentumTracker = new Stack<HardwareTypes>();
        hardwareTypeToMomentumMap = new Dictionary<HardwareTypes, int>();

        Debug.Log("Momentum necessary for point: " + momentumRequiredForNextPoint);
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += ClearMomentum;
        GlobalEventEmitter.OnEntityDied += HandleEntityDied;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= ClearMomentum;
        GlobalEventEmitter.OnEntityDied -= HandleEntityDied;
    }

    #region momentum event handlers

    static void HandleEntityDied(string entityID, int quantity)
    {
        AddMomentum(quantity);
    }

    static void AddMomentum(int quantityToAdd)
    {
        progressTowardNextMomentum += quantityToAdd;

        while (progressTowardNextMomentum >= momentumRequiredForNextPoint)
        {
            progressTowardNextMomentum %= momentumRequiredForNextPoint;
            unassignedAvailableMomentumPoints++;

            OnAvailableMomentumPointsUpdated(unassignedAvailableMomentumPoints);
        }
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

    public static void AssignMomentumPointToHardware(HardwareTypes hardwareType)
    {
        if (unassignedAvailableMomentumPoints == 0)
        {
            Debug.LogError("Attempting to assign momentum points; no momentum points to assign.");
            return;
        }
        if (!hardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            hardwareTypeToMomentumMap[hardwareType] = 0;
        }

        hardwareTypeToMomentumMap[hardwareType]++;
        assignedMomentumTracker.Push(hardwareType);
        OnAssignedMomentumPointsUpdated(hardwareTypeToMomentumMap);

        unassignedAvailableMomentumPoints--;
        OnAvailableMomentumPointsUpdated(unassignedAvailableMomentumPoints);
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
        OnAssignedMomentumPointsUpdated(hardwareTypeToMomentumMap);
    }

    static void ClearMomentum(InventoryData inventory)
    {
        hardwareTypeToMomentumMap.Clear();
        assignedMomentumTracker.Clear();
        OnAssignedMomentumPointsUpdated(hardwareTypeToMomentumMap);
    }
    #endregion
}
