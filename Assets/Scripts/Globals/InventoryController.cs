using UnityEngine;

public class InventoryController {

    public delegate void PassInventoryDelegate(InventoryData inventory);
    public static PassInventoryDelegate OnInventoryUpdated;

    static InventoryData _inventory;
    public static InventoryData Inventory {
        get
        {
            if (_inventory == null)
            {
                if (MasterSerializer.CanLoadInventoryData())
                {
                    _inventory = MasterSerializer.LoadInventoryData();
                }
                else
                {
                    _inventory = new InventoryData();
                }
            }

            return _inventory;
        }
    }

    public static void DiscoverHardware(HardwareType hardwareType)
    {
        Inventory.ObtainHardwareType(hardwareType);
        OnInventoryUpdated(Inventory);
    }

    public static bool HasDiscoveredHardware(HardwareType hardwareType)
    {
        return Inventory.ObtainedHardware[hardwareType];
    }

    public static HardwareType[] GetEquippedActiveHardware()
    {
        return Inventory.EquippedActiveHardware;
    }

    public static HardwareType[] GetEquippedPassiveHardware()
    {
        return Inventory.EquippedPassiveHardware;
    }

    #region equipping/unequipping

    public static void EquipActiveHardware(int slot, HardwareType hardwareType)
    {
        if (slot == 0 || slot == 1)
        {
            Debug.LogError("Trying to equip in Parry or Blink slot");
            return;
        }
        Inventory.EquippedActiveHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void EquipPassiveHardware(int slot, HardwareType hardwareType)
    {
        Inventory.EquippedPassiveHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void UnequipActiveHardware(int slot)
    {
        Inventory.EquippedActiveHardware[slot] = HardwareType.None;

        OnInventoryUpdated(Inventory);
        UnequipPassiveHardware(slot);
    }

    public static void UnequipPassiveHardware(int slot)
    {
        Inventory.EquippedPassiveHardware[slot] = HardwareType.None;

        OnInventoryUpdated(Inventory);
    }

    #endregion
}
