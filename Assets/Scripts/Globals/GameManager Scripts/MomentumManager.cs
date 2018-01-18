using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumManager : MonoBehaviour {

    public delegate void MomentumUpdatedDelegate(MomentumData momentumData);
    public static MomentumUpdatedDelegate OnMomentumUpdated;

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

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += ClearMomentum;
        GlobalEventEmitter.OnGameStateEvent += HandleEntityDied;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= ClearMomentum;
        GlobalEventEmitter.OnGameStateEvent -= HandleEntityDied;
    }

    #region momentum event handlers

    static void HandleEntityDied(GlobalConstants.GameStateEvents stateEvent, string quantity)
    {
        if (stateEvent == GlobalConstants.GameStateEvents.EntityDied)
        {
            int quantityInt = Int32.Parse(quantity);
            AddMomentum(quantityInt);
        }
    }

    static void AddMomentum(int quantityToAdd)
    {
        CurrentMomentumData.AddMomentum(quantityToAdd);

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

        CurrentMomentumData.UnassignedAvailableMomentumPoints--;
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
        OnMomentumUpdated(CurrentMomentumData);
    }

    static void ClearMomentum(InventoryData inventory)
    {
        _currentMomentumData = new MomentumData();
        OnMomentumUpdated(CurrentMomentumData);
    }
    #endregion
}
