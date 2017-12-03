using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware parryGear;
    public IHardware ParryGear { get { return parryGear; } }
    IHardware blinkGear;
    public IHardware BlinkGear { get { return blinkGear; } }

    delegate void PassiveHardwareDelegate(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject);
    PassiveHardwareDelegate passiveHardware_Parry;
    PassiveHardwareDelegate passiveHardware_Blink;

    PassiveHardwareDelegate passiveHardware_Slot1;
    PassiveHardwareDelegate passiveHardware_Slot2;

    IHardware equippedGear_Slot1;
    public IHardware EquippedGear_Slot1
    {
        get
        {
            return equippedGear_Slot1;
        }
    }

    IHardware equippedGear_Slot2;
    public IHardware EquippedGear_Slot2
    {
        get
        {
            return equippedGear_Slot2;
        }
    }

    void Start()
    {
        parryGear = GetComponent<ParryHardware>() as IHardware;
        blinkGear = GetComponent<BlinkHardware>() as IHardware;

        EquipActiveGear_Slot1(typeof(NullifierHardware));
        EquipActiveGear_Slot2(typeof(RiposteHardware));

        passiveHardware_Parry += equippedGear_Slot1.ApplyPassiveHardware;
        passiveHardware_Blink += equippedGear_Slot1.ApplyPassiveHardware;
    }

    void EquipActiveGear_Slot1(Type newHardware)
    {
        equippedGear_Slot1 = gameObject.AddComponent(newHardware) as IHardware;
    }

    void EquipActiveGear_Slot2(Type newHardware)
    {
        equippedGear_Slot2 = gameObject.AddComponent(newHardware) as IHardware;
    }

    public void ApplyParryPassiveHardwareToBullet(GameObject bullet)
    {
        passiveHardware_Parry(HardwareTypes.Parry, parryGear, bullet);
    }

    public void ApplyPassiveHardwareToBlink(GameObject player)
    {
        passiveHardware_Blink(HardwareTypes.Blink, blinkGear, player);
    }
}
