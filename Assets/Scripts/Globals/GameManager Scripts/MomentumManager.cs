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
        InventoryController.OnInventoryUpdated += ResetMomentum;
        GlobalEventEmitter.OnGameStateEvent += HandleEntityDied;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= ResetMomentum;
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
    public static int GetMomentumPointsByHardwareType(HardwareType hardwareType)
    {
        if (!CurrentMomentumData.HardwareTypeToMomentumMap.ContainsKey(hardwareType))
        {
            CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType] = 0;
        }

        return CurrentMomentumData.HardwareTypeToMomentumMap[hardwareType];
    }

    public static void AssignMomentumPointToHardware(HardwareType hardwareType)
    {
        if (CurrentMomentumData.UnassignedAvailableMomentumPoints == 0)
        {
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

    public static void RemoveLastMomentumPoint()
    {
        ClearProgressTowardNextMomentum();
        if (CurrentMomentumData.AssignedMomentumTracker.Count <= 0f)
        {
            Debug.Log("Trying to undo last momentum assignment with no momentum assigned.");
            return;
        }
        HardwareType lastHardwareTypeIncremented = CurrentMomentumData.AssignedMomentumTracker.Pop();

        CurrentMomentumData.HardwareTypeToMomentumMap[lastHardwareTypeIncremented]--;
        OnMomentumUpdated(CurrentMomentumData);
        GlobalEventEmitter.OnGameStateEvent(GlobalConstants.GameStateEvents.MomentumLost);
    }

    static void ClearProgressTowardNextMomentum()
    {
        CurrentMomentumData.ProgressTowardNextMomentum = 0;
        OnMomentumUpdated(CurrentMomentumData);
    }

    static void ResetMomentum(InventoryData inventory)
    {
        ResetMomentum();
    }

    public static void ResetMomentum()
    {
        _currentMomentumData = new MomentumData();
        OnMomentumUpdated(CurrentMomentumData);
    }
    #endregion
}
