using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class InventoryData {

    public Dictionary<HardwareType, bool> ObtainedHardware;
    public Dictionary<RenewableTypes, bool> ObtainedRenewables;

    public Dictionary<HardwareType, List<bool>> DiscoveredHardwareSubtypes;

    public HardwareType[] EquippedActiveHardware;
    public Type[] EquippedActiveSubtypes;

    public HardwareType[] EquippedPassiveHardware;
    public RenewableTypes EquippedRenewable;

    public InventoryData()
    {
        ObtainedHardware = new Dictionary<HardwareType, bool>();
        ObtainedRenewables = new Dictionary<RenewableTypes, bool>();
        DiscoveredHardwareSubtypes = new Dictionary<HardwareType, List<bool>>();

        foreach (var value in Enum.GetValues(typeof(HardwareType)))
        {
            HardwareType hardwareType = (HardwareType)value;
            if (hardwareType != HardwareType.None)
            {
                ObtainedHardware[hardwareType] = false;
                DiscoveredHardwareSubtypes[hardwareType] = new List<bool>(3)
                {
                    false,
                    false,
                    false
                };
            }
        }

        foreach (var value in Enum.GetValues(typeof(RenewableTypes)))
        {
            RenewableTypes renewableType = (RenewableTypes)value;
            if (renewableType != RenewableTypes.None)
            {
                ObtainedRenewables[renewableType] = false;
            }
        }

        ObtainHardwareType(HardwareType.Parry);
        ObtainHardwareType(HardwareType.Blink);

        ObtainRenewableType(RenewableTypes.NoetherFrictionConverter);
        ObtainRenewableType(RenewableTypes.GravesandeImpulseAdapter);

        EquippedActiveHardware = new HardwareType[4]
        {
            HardwareType.Parry,
            HardwareType.Blink,
            HardwareType.None,
            HardwareType.None
        };

        EquippedActiveSubtypes = new Type[4]
        {
            typeof(StandardIssueParryHardwareData),
            typeof(StandardIssueBlinkHardwareData),
            null,
            null
        };

        EquippedPassiveHardware = new HardwareType[4];
        EquippedRenewable = RenewableTypes.NoetherFrictionConverter;
    }

    public void ObtainHardwareType(HardwareType hardwareType)
    {
        ObtainedHardware[hardwareType] = true;
        DiscoveredHardwareSubtypes[hardwareType][0] = true;
    }

    public void ObtainRenewableType(RenewableTypes renewableType)
    {
        ObtainedRenewables[renewableType] = true;
    }
}