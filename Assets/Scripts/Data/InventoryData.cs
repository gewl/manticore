using System.Collections;
using System.Collections.Generic;

public class InventoryData
{
    public Dictionary<HardwareTypes, bool> obtainedInventory;
    public HardwareTypes[] activeHardware;
    public HardwareTypes[] passiveHardware;

    public InventoryData()
    {
        obtainedInventory = new Dictionary<HardwareTypes, bool>();
        obtainedInventory[HardwareTypes.Parry] = true;
        obtainedInventory[HardwareTypes.Blink] = true;

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
