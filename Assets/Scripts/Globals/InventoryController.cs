using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    public delegate void PassInventoryDelegate(InventoryData inventory);
    public static PassInventoryDelegate OnInventoryUpdated;

    static InventoryData _inventory;
    public static InventoryData Inventory {
        get
        {
            if (_inventory == null)
            {
                _inventory = new InventoryData();
            }

            return _inventory;
        }
    }

    public static void DiscoverHardware(HardwareTypes hardwareType)
    {
        Inventory.obtainedInventory[hardwareType] = true;
    }

    public static bool HasDiscoveredHardware(HardwareTypes hardwareType)
    {
        return Inventory.obtainedInventory[hardwareType];
    }

    public static HardwareTypes[] GetEquippedActiveHardware()
    {
        return Inventory.activeHardware;
    }

    public static HardwareTypes[] GetEquippedPassiveHardware()
    {
        return Inventory.passiveHardware;
    }

    #region equipping/unequipping

    public static void EquipActiveHardware(int slot, HardwareTypes hardwareType)
    {
        if (slot == 0 || slot == 1)
        {
            Debug.LogError("Trying to equip in Parry or Blink slot");
            return;
        }
        Inventory.activeHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void EquipPassiveHardware(int slot, HardwareTypes hardwareType)
    {
        Inventory.passiveHardware[slot] = hardwareType;

        OnInventoryUpdated(Inventory);
    }

    public static void UnequipActiveHardware(int slot)
    {
        Inventory.activeHardware[slot] = HardwareTypes.None;

        OnInventoryUpdated(Inventory);
    }

    public static void UnequipPassiveHardware(int slot)
    {
        Inventory.passiveHardware[slot] = HardwareTypes.None;

        OnInventoryUpdated(Inventory);
    }

    #endregion
}
