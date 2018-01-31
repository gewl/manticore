﻿using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class MomentumData {

    public Dictionary<HardwareTypes, int> HardwareTypeToMomentumMap;
    // This tracks in what order & to which Hardware momentum was assigned.
    // When player loses momentum, the last assigned momentum is removed.
    // Count also used to track total assigned momentum.
    public Stack<HardwareTypes> AssignedMomentumTracker;

    public int ProgressTowardNextMomentum = 0;
    public int TotalMomentumPoints { get { return AssignedMomentumTracker.Count + UnassignedAvailableMomentumPoints; } }
    public int MomentumRequiredForNextPoint
    {
        get
        {
            return (TotalMomentumPoints + 1) * 5;
        }
    }

    public int UnassignedAvailableMomentumPoints = 0;

    public MomentumData()
    {
        HardwareTypeToMomentumMap = new Dictionary<HardwareTypes, int>();
        AssignedMomentumTracker = new Stack<HardwareTypes>();
    }

    public void AddMomentum(int quantityToAdd)
    {
        ProgressTowardNextMomentum += quantityToAdd;

        while (ProgressTowardNextMomentum >= MomentumRequiredForNextPoint)
        {
            ProgressTowardNextMomentum -= MomentumRequiredForNextPoint;
            UnassignedAvailableMomentumPoints++;
            GlobalEventEmitter.OnGameStateEvent(GlobalConstants.GameStateEvents.NewMomentumPoint);
        }
    }
}
