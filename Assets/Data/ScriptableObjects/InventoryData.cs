using System;
using System.Collections.Generic;

[Serializable]
public class InventoryData {

    public Dictionary<HardwareTypes, bool> ObtainedHardware;
    public Dictionary<RenewableTypes, bool> ObtainedRenewables;

    public Dictionary<HardwareTypes, List<bool>> DiscoveredHardwareSubtypes;
    public HardwareTypes[] ActiveHardware;
    public HardwareTypes[] PassiveHardware;

    public InventoryData()
    {
        ObtainedHardware = new Dictionary<HardwareTypes, bool>();
        ObtainedRenewables = new Dictionary<RenewableTypes, bool>();
        DiscoveredHardwareSubtypes = new Dictionary<HardwareTypes, List<bool>>();

        foreach (var value in Enum.GetValues(typeof(HardwareTypes)))
        {
            HardwareTypes hardwareType = (HardwareTypes)value;
            if (hardwareType != HardwareTypes.None)
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

        ObtainHardwareType(HardwareTypes.Parry);
        ObtainHardwareType(HardwareTypes.Blink);
        ObtainHardwareType(HardwareTypes.Riposte);
        ObtainHardwareType(HardwareTypes.Nullify);

        ActiveHardware = new HardwareTypes[4]
        {
            HardwareTypes.Parry,
            HardwareTypes.Blink,
            HardwareTypes.None,
            HardwareTypes.None
        };

        PassiveHardware = new HardwareTypes[4];
    }

    public void ObtainHardwareType(HardwareTypes hardwareType)
    {
        ObtainedHardware[hardwareType] = true;
        DiscoveredHardwareSubtypes[hardwareType][0] = true;
    }

    public void ObtainRenewableType(RenewableTypes renewableType)
    {
        ObtainedRenewables[renewableType] = true;
    }
}