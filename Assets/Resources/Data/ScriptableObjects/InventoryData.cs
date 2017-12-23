using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public Dictionary<HardwareTypes, bool> obtainedHardware;
    public Dictionary<HardwareTypes, List<bool>> discoveredHardwareSubtypes;
    public HardwareTypes[] activeHardware;
    public HardwareTypes[] passiveHardware;

    public InventoryData()
    {
        obtainedHardware = new Dictionary<HardwareTypes, bool>();
        discoveredHardwareSubtypes = new Dictionary<HardwareTypes, List<bool>>();

        foreach (var value in Enum.GetValues(typeof(HardwareTypes)))
        {
            HardwareTypes hardwareType = (HardwareTypes)value;
            if (hardwareType != HardwareTypes.None)
            {
                obtainedHardware[hardwareType] = false;
                discoveredHardwareSubtypes[hardwareType] = new List<bool>(3)
                {
                    false,
                    false,
                    false
                };
            }
        }

        ObtainHardwareType(HardwareTypes.Parry);
        ObtainHardwareType(HardwareTypes.Blink);

        activeHardware = new HardwareTypes[4]
        {
            HardwareTypes.Parry,
            HardwareTypes.Blink,
            HardwareTypes.None,
            HardwareTypes.None
        };

        passiveHardware = new HardwareTypes[4];
    }

    public void ObtainHardwareType(HardwareTypes hardwareType)
    {
        obtainedHardware[hardwareType] = true;
        discoveredHardwareSubtypes[hardwareType][0] = true;
    }
}