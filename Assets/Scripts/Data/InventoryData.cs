using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public Dictionary<HardwareTypes, bool> obtainedInventory;
    public HardwareTypes[] activeHardware;
    public HardwareTypes[] passiveHardware;

    public InventoryData()
    {
        obtainedInventory = new Dictionary<HardwareTypes, bool>();

        foreach (var value in Enum.GetValues(typeof(HardwareTypes)))
        {
            HardwareTypes hardwareType = (HardwareTypes)value;
            if (hardwareType != HardwareTypes.None)
            {
                obtainedInventory[hardwareType] = false;
            }
        }

        obtainedInventory[HardwareTypes.Parry] = true;
        obtainedInventory[HardwareTypes.Blink] = true;
        obtainedInventory[HardwareTypes.Nullify] = true;
        obtainedInventory[HardwareTypes.Riposte] = true;

        activeHardware = new HardwareTypes[4]
        {
            HardwareTypes.Parry,
            HardwareTypes.Blink,
            HardwareTypes.None,
            HardwareTypes.None
        };

        passiveHardware = new HardwareTypes[4];
    }
}
