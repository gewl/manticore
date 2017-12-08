using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    InventoryData inventory;

    IHardware parryGear;
    public IHardware ParryGear { get { return parryGear; } }
    IHardware blinkGear;
    public IHardware BlinkGear { get { return blinkGear; } }
    IHardware activeHardware_Slot2;
    public IHardware EquippedGear_Slot2 { get { return activeHardware_Slot2; } }
    IHardware activeHardware_Slot3;
    public IHardware EquippedGear_Slot3 { get { return activeHardware_Slot3; } }

    List<IHardware> equippedPassiveHardware;

    delegate void ApplyPassiveHardwareDelegate(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject);
    ApplyPassiveHardwareDelegate passiveHardware_Parry;
    ApplyPassiveHardwareDelegate passiveHardware_Blink;
    ApplyPassiveHardwareDelegate passiveHardware_Slot2;
    ApplyPassiveHardwareDelegate passiveHardware_Slot3;

    IHardware[] activeHardware;
    ApplyPassiveHardwareDelegate[] passiveHardwareDelegates;

    public delegate void PassActiveHardwareDelegate (ref IHardware[] activeHardware);
    public PassActiveHardwareDelegate activeHardwareUpdated;

    void Start()
    {
        inventory = new InventoryData();

        equippedPassiveHardware = new List<IHardware>(4)
        {
            null,
            null,
            null,
            null
        };

        parryGear = GetComponent<ParryHardware>() as IHardware;
        blinkGear = GetComponent<BlinkHardware>() as IHardware;

        activeHardware = new IHardware[4]
        {
            parryGear,
            blinkGear,
            activeHardware_Slot2,
            activeHardware_Slot3
        };

        EquipHardwareGear_Slot2(typeof(NullifierHardware));
        EquipHardwareGear_Slot3(typeof(RiposteHardware));

        passiveHardwareDelegates = new ApplyPassiveHardwareDelegate[4]
        {
            passiveHardware_Parry,
            passiveHardware_Blink,
            passiveHardware_Slot2,
            passiveHardware_Slot3
        };

        EquipAndAssignPassiveHardware(0, typeof(RiposteHardware));
        EquipAndAssignPassiveHardware(3, typeof(NullifierHardware));
    }

    void EquipHardwareGear_Slot2(Type newHardware)
    {
        activeHardware_Slot2 = gameObject.AddComponent(newHardware) as IHardware;
        activeHardware[2] = activeHardware_Slot2;
        activeHardwareUpdated(ref activeHardware);
    }

    void EquipHardwareGear_Slot3(Type newHardware)
    {
        activeHardware_Slot3 = gameObject.AddComponent(newHardware) as IHardware;
        activeHardware[3] = activeHardware_Slot3;
        activeHardwareUpdated(ref activeHardware);
    }

    void UnequipHardwareGear(int activeHardwareSlot)
    {
        if (activeHardwareSlot == 0 || activeHardwareSlot == 1)
        {
            Debug.LogError("Trying to unequip Blink or Parry.");
            return;
        }

        UnequipPassiveHardware(activeHardwareSlot);
        Type activeGearType = activeHardware[activeHardwareSlot].GetType();
        Destroy(GetComponent(activeGearType));

        activeHardwareUpdated(ref activeHardware);
    }

    public void EquipAndAssignPassiveHardware(int activeSlot, Type newHardware)
    {
        if (equippedPassiveHardware[activeSlot] != null)
        {
            Debug.LogError("Already passive hardware in that slot");
            return;
        }
        IHardware passiveHardware = gameObject.AddComponent(newHardware) as IHardware;
        passiveHardwareDelegates[activeSlot] += passiveHardware.ApplyPassiveHardware;

        equippedPassiveHardware[activeSlot] = passiveHardware;
    }

    public void UnequipPassiveHardware(int activeSlot)
    {
        IHardware passiveHardware = equippedPassiveHardware[activeSlot];
        passiveHardwareDelegates[activeSlot] -= passiveHardware.ApplyPassiveHardware;
        Type passiveHardwareType = passiveHardware.GetType();
        Destroy(GetComponent(passiveHardwareType));

        equippedPassiveHardware[activeSlot] = null;
    }

    public void ApplyPassiveHardware(Type newHardware, GameObject subject)
    {
        int hardwareIndex = -1;
        IHardware hardware = activeHardware[0];
        // Basically "find hardware of type"
        for (int i = 0; i < activeHardware.Length; i++)
        {
            hardware = activeHardware[i];
            if (hardware.GetType() == newHardware)
            {
                hardwareIndex = i;
                break;
            } 
        }
        if (hardwareIndex == -1)
        {
            Debug.LogError("Hardware not found in ApplyPassiveHardware.");
        }

        ApplyPassiveHardwareDelegate passiveHardwareDelegate = passiveHardwareDelegates[hardwareIndex];

        if (passiveHardwareDelegate != null)
        {
            passiveHardwareDelegate(hardware.Type, hardware, subject);
        }
    }
}
