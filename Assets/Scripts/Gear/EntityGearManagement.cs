using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware parryGear;
    public IHardware ParryGear { get { return parryGear; } }
    IHardware blinkGear;
    public IHardware BlinkGear { get { return blinkGear; } }
    IHardware equippedGear_Slot2;
    public IHardware EquippedGear_Slot2 { get { return equippedGear_Slot2; } }
    IHardware equippedGear_Slot3;
    public IHardware EquippedGear_Slot3 { get { return equippedGear_Slot3; } }

    delegate void PassiveHardwareDelegate(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject);

    PassiveHardwareDelegate passiveHardware_Parry;
    PassiveHardwareDelegate passiveHardware_Blink;
    PassiveHardwareDelegate passiveHardware_Slot2;
    PassiveHardwareDelegate passiveHardware_Slot3;

    IHardware[] activeHardware;
    PassiveHardwareDelegate[] passiveHardwareDelegates;

    void Start()
    {
        parryGear = GetComponent<ParryHardware>() as IHardware;
        blinkGear = GetComponent<BlinkHardware>() as IHardware;

        EquipActiveGear_Slot2(typeof(NullifierHardware));
        EquipActiveGear_Slot3(typeof(RiposteHardware));

        activeHardware = new IHardware[4]
        {
            parryGear,
            blinkGear,
            equippedGear_Slot2,
            equippedGear_Slot3
        };

        passiveHardwareDelegates = new PassiveHardwareDelegate[4]
        {
            passiveHardware_Parry,
            passiveHardware_Blink,
            passiveHardware_Slot2,
            passiveHardware_Slot3
        };

        EquipAndAssignPassiveHardware(1, typeof(NullifierHardware));
        EquipAndAssignPassiveHardware(0, typeof(RiposteHardware));
    }

    void EquipActiveGear_Slot2(Type newHardware)
    {
        equippedGear_Slot2 = gameObject.AddComponent(newHardware) as IHardware;
    }

    void EquipActiveGear_Slot3(Type newHardware)
    {
        equippedGear_Slot3 = gameObject.AddComponent(newHardware) as IHardware;
    }

    public void EquipAndAssignPassiveHardware(int activeSlot, Type newHardware)
    {
        IHardware passiveHardware = gameObject.AddComponent(newHardware) as IHardware;
        passiveHardwareDelegates[activeSlot] += passiveHardware.ApplyPassiveHardware;
    }

    public void ApplyPassiveHardware(Type newHardware, GameObject subject)
    {
        int hardwareIndex = -1;
        IHardware hardware = activeHardware[0];
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

        PassiveHardwareDelegate passiveHardwareDelegate = passiveHardwareDelegates[hardwareIndex];

        if (passiveHardwareDelegate != null)
        {
            passiveHardwareDelegate(hardware.Type, hardware, subject);
        }
    }

    //public void ApplyParryPassiveHardwareToBullet(GameObject bullet)
    //{
    //    if (passiveHardware_Parry != null)
    //    {
    //        passiveHardware_Parry(HardwareTypes.Parry, parryGear, bullet);
    //    }
    //}

    //public void ApplyPassiveHardwareToBlink(GameObject player)
    //{
    //    if (passiveHardware_Blink != null)
    //    {
    //        passiveHardware_Blink(HardwareTypes.Blink, blinkGear, player);
    //    }
    //}

    //public void ApplyPassiveHardwareToSlot2(GameObject subject)
    //{
    //    if (passiveHardware_Slot2 != null)
    //    {
    //        passiveHardware_Slot2(equippedGear_Slot2.Type, equippedGear_Slot2, subject);
    //    }
    //}

    //public void ApplyPassiveHardwareToSlot3(GameObject subject)
    //{
    //    if (passiveHardware_Slot3 != null)
    //    {
    //        passiveHardware_Slot3(equippedGear_Slot3.Type, equippedGear_Slot3, subject);
    //    }
    //}
}
