using System.Collections;
using System.Collections.Generic;

public class InventoryData
{
    public IHardware[] unequippedInventory;
    public IHardware[] activeInventory;
    public IHardware[] passiveInventory;

    public InventoryData()
    {
        unequippedInventory = new IHardware[9];

        activeInventory = new IHardware[4]
        {
            new ParryHardware(),
            new BlinkHardware(),
            null,
            null
        };
        passiveInventory = new IHardware[4];
    }
}
