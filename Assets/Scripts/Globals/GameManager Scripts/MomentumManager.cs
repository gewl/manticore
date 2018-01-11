using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumManager : MonoBehaviour {

    public delegate void MomentumUpdatedDelegate(MomentumData momentumData);
    public static MomentumUpdatedDelegate OnMomentumUpdated;
    //public delegate void MomentumPointsUpdatedDelegate(int availablePoints);
    //public static MomentumPointsUpdatedDelegate OnAvailableMomentumPointsUpdated;

    //public delegate void AssignedMomentumPointsUpdatedDelegate(Dictionary<HardwareTypes, int> hardwareTypeToMomentumMap);
    //public static AssignedMomentumPointsUpdatedDelegate OnAssignedMomentumPointsUpdated;

    static MomentumData _currentMomentumData;
    public static MomentumData CurrentMomentumData
    {
        get
        {
            if (_currentMomentumData == null)
            {
                if (MasterSerializer.CanLoadMomentumData())
                {
                    _currentMomentumData = MasterSerializer.LoadMomentumData();
                }
                else
                {
                    _currentMomentumData = new MomentumData();
                }
            }

            return _currentMomentumData;
        }
    }

    static float progressTowardNextMomentum { get { return CurrentMomentumData.ProgressTowardNextMomentum; } }

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
        CurrentMomentumData.AddMomentum(quantityToAdd);

        //OnAvailableMomentumPointsUpdated(CurrentMomentumData.UnassignedAvailableMomentumPoints);
        OnMomentumUpdated(CurrentMomentumData);
    }

    #endregion

    #region point manipulation
    public static int GetMomentumPointsByHardwareType(HardwareTypes hardwareType)
    {
        if (!CurrentMomentumData.HardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType] = 0;
        }

        return CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType];
    }

    public static void AssignMomentumPointToHardware(HardwareTypes hardwareType)
    {
        if (CurrentMomentumData.UnassignedAvailableMomentumPoints == 0)
        {
            Debug.LogError("Attempting to assign momentum points; no momentum points to assign.");
            return;
        }
        if (!CurrentMomentumData.HardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType] = 0;
        }

        CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType]++;
        CurrentMomentumData.AssignedMomentumTracker.Push(hardwareType);
        //OnAssignedMomentumPointsUpdated(CurrentMomentumData.HardwareTypeToMomentumMap);

        CurrentMomentumData.UnassignedAvailableMomentumPoints--;
        //OnAvailableMomentumPointsUpdated(CurrentMomentumData.UnassignedAvailableMomentumPoints);
        OnMomentumUpdated(CurrentMomentumData);
    }

    static void RemoveLastMomentumPoint()
    {
        if (CurrentMomentumData.AssignedMomentumTracker.Count <= 0f)
        {
            Debug.Log("Trying to undo last momentum assignment with no momentum assigned.");
            return;
        }
        HardwareTypes lastHardwareTypeIncremented = CurrentMomentumData.AssignedMomentumTracker.Pop();

        CurrentMomentumData.HardwareTypeToMomentumMap[lastHardwareTypeIncremented]--;
        //OnAssignedMomentumPointsUpdated(CurrentMomentumData.HardwareTypeToMomentumMap);
        OnMomentumUpdated(CurrentMomentumData);
    }

    static void ClearMomentum(InventoryData inventory)
    {
        _currentMomentumData = new MomentumData();
        //OnAssignedMomentumPointsUpdated(CurrentMomentumData.HardwareTypeToMomentumMap);
        OnMomentumUpdated(CurrentMomentumData);
    }
    #endregion
}
