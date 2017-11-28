using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware EquippedGear_Slot1;

    public int StaminaCost_Slot1
    {
        get
        {
            return EquippedGear_Slot1.UpdatedStaminaCost;
        }
    }

    void Start()
    {
        EquippedGear_Slot1 = gameObject.AddComponent(typeof(NullifierComponent)) as IHardware;
    }


    public void UseGear_Slot1()
    {
        EquippedGear_Slot1.UseActiveHardware();
    }
}
