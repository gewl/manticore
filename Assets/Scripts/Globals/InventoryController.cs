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

    public static void DiscoverHardware(HardwareTypes hardwareType)
    {
        Inventory.ObtainHardwareType(hardwareType);
        OnInventoryUpdated(Inventory);
    }

    public static bool HasDiscoveredHardware(HardwareTypes hardwareType)
    {
        return Inventory.ObtainedHardware[hardwareType];
    }

    public static HardwareTypes[] GetEquippedActiveHardware()
    {
        return Inventory.ActiveHardware;
    }

    public static HardwareTypes[] GetEquippedPassiveHardware()
    {
        return Inventory.PassiveHardware;
    }

    #region equipping/unequipping

    public static void EquipActiveHardware(int slot, HardwareTypes hardwareType)
    {
        if (slot == 0 || slot == 1)
        {
            Debug.LogError("Trying to equip in Parry or Blink slot");
            return;
        }
        Inventory.ActiveHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void EquipPassiveHardware(int slot, HardwareTypes hardwareType)
    {
        Inventory.PassiveHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void UnequipActiveHardware(int slot)
    {
        Inventory.ActiveHardware[slot] = HardwareTypes.None;

        OnInventoryUpdated(Inventory);
        UnequipPassiveHardware(slot);
    }

    public static void UnequipPassiveHardware(int slot)
    {
        Inventory.PassiveHardware[slot] = HardwareTypes.None;

        OnInventoryUpdated(Inventory);
    }

    #endregion
}
