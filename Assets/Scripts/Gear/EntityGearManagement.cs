using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware parryGear;
    public IHardware ParryGear { get { return parryGear; } }
    IHardware blinkGear;
    public IHardware BlinkGear { get { return blinkGear; } }

    List<IHardware> passiveHardwareOnParry;

    IHardware equippedGear_Slot1;
    public IHardware EquippedGear_Slot1
    {
        get
        {
            return equippedGear_Slot1;
        }
    }

    void Start()
    {
        parryGear = GetComponent<ParryHardware>() as IHardware;
        blinkGear = GetComponent<BlinkHardware>() as IHardware;
        equippedGear_Slot1 = gameObject.AddComponent(typeof(NullifierHardware)) as IHardware;

        passiveHardwareOnParry = new List<IHardware>();
        passiveHardwareOnParry.Add(GetComponent<NullifierHardware>());
    }

    public void ApplyParryPassiveHardwareToBullet(GameObject bullet)
    {
        for (int i = 0; i < passiveHardwareOnParry.Count; i++)
        {
            IHardware passiveHardware = passiveHardwareOnParry[i];

            passiveHardware.ApplyPassiveHardware(HardwareTypes.Parry, bullet);
        }
    }
}
