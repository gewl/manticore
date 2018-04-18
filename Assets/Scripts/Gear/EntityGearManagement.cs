// Disable warnings about PassiveHardwareDelegates not being assigned to;
// other components subscribe to them in their OnEnable methods.
#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware[] activeHardware;
    List<IHardware> passiveHardware;
    ApplyPassiveHardwareDelegate[] passiveHardwareDelegates;

    IRenewable equippedRenewable;

    public IHardware ParryGear { get { return activeHardware[0]; } }
    public IHardware BlinkGear { get { return activeHardware[1]; } }
    public IHardware EquippedGear_Slot2 { get { return activeHardware[2]; } }
    public IHardware EquippedGear_Slot3 { get { return activeHardware[3]; } }

    public IRenewable EquippedRenewable { get { return equippedRenewable; } }

    delegate void ApplyPassiveHardwareDelegate(HardwareType activeHardwareType, IHardware activeHardware, GameObject subject);
    ApplyPassiveHardwareDelegate passiveHardware_Parry;
    ApplyPassiveHardwareDelegate passiveHardware_Blink;
    ApplyPassiveHardwareDelegate passiveHardware_Slot2;
    ApplyPassiveHardwareDelegate passiveHardware_Slot3;

    public delegate void PassActiveHardware(ref IHardware[] activeHardware);
    public PassActiveHardware activeHardwareUpdated;

    public delegate void PassActiveRenewable(ref IRenewable activeRenewable);
    public PassActiveRenewable activeRenewableUpdated;

    private void Awake()
    {
        InitializeGear();
    }

    void Start()
    {
        UpdateGear(InventoryController.Inventory);
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += UpdateGear;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= UpdateGear;
    }

    void InitializeGear()
    {
        IHardware parryGear = GetComponent<ParryHardware>() as IHardware;
        IHardware blinkGear = GetComponent<BlinkHardware>() as IHardware;

        activeHardware = new IHardware[4];
        activeHardware[0] = parryGear;
        activeHardware[1] = blinkGear;
        activeHardware[2] = null;
        activeHardware[3] = null;

        passiveHardware = new List<IHardware>(4)
        {
            null,
            null,
            null,
            null
        };

        passiveHardwareDelegates = new ApplyPassiveHardwareDelegate[4]
        {
            passiveHardware_Parry,
            passiveHardware_Blink,
            passiveHardware_Slot2,
            passiveHardware_Slot3
        };
    }

    public void UpdateGear(InventoryData inventory)
    {
        // Remove components for all old hardware. 
        // It iterates twice (rather than removing & adding in one iteration) to prevent
        // sequencing causing a component required for 'new' hardware to be deleted because it was
        // used for 'old' hardware.
        // Right now this is naive--it doesn't diff against new hardware.
        if (activeHardware[2] != null)
        {
            ClearActiveHardware(2);
        }
        if (activeHardware[3] != null)
        {
            ClearActiveHardware(3);
        }

        for (int i = 0; i < passiveHardware.Count; i++)
        {
            IHardware passiveHardwareComponent = passiveHardware[i];
            if (passiveHardwareComponent != null)
            {
                ClearPassiveHardware(i);
            }
        }

        // Add components for new hardware.
        for (int i = 0; i < inventory.EquippedActiveHardware.Length; i++)
        {
            HardwareType hardwareType = inventory.EquippedActiveHardware[i];
            if (hardwareType == HardwareType.None)
            {
                continue;
            }
            if (i != 0 && i != 1)
            {
                GenerateActiveHardwareComponent(hardwareType, i);
            }
            AssignSubtypeData(i);
        }

        for (int i = 0; i < inventory.EquippedPassiveHardware.Length; i++)
        {
            HardwareType hardwareType = inventory.EquippedPassiveHardware[i];
            if (hardwareType == HardwareType.None)
            {
                continue;
            }
            GeneratePassiveHardwareComponent(hardwareType, i);
        }

        if (activeHardwareUpdated != null)
        {
            activeHardwareUpdated(ref activeHardware);
        }

        if (equippedRenewable != null)
        {
            ClearRenewable();
        }
        if (inventory.EquippedRenewable != RenewableTypes.None)
        {
            GenerateRenewable(inventory.EquippedRenewable);
            activeRenewableUpdated(ref equippedRenewable);
        }
    }

    Type GetHardwareType(HardwareType hardwareType)
    {
        switch (hardwareType)
        {
            case HardwareType.None:
                return null;
            case HardwareType.Parry:
                return typeof(ParryHardware);
            case HardwareType.Blink:
                return typeof(BlinkHardware);
            case HardwareType.Nullify:
                return typeof(NullifyHardware);
            case HardwareType.Fracture:
                return typeof(FractureHardware);
            case HardwareType.Yank:
                return typeof(YankHardware);
            default:
                return null;
        }
    }

    Type GetRenewableType(RenewableTypes renewableType)
    {
        switch (renewableType)
        {
            case RenewableTypes.None:
                return null;
            case RenewableTypes.NoetherFrictionConverter:
                return typeof(NoetherFrictionConverter);
            case RenewableTypes.GravesandeImpulseAdapter:
                return null;
            default:
                return null;
        }
    }
    
    void GenerateActiveHardwareComponent(HardwareType newHardwareType, int index)
    {
        Type newHardware = GetHardwareType(newHardwareType);
        activeHardware[index] = gameObject.AddComponent(newHardware) as IHardware;
    }

    void AssignSubtypeData(int index)
    {
        Type hardwareSubtype = InventoryController.Inventory.EquippedActiveSubtypes[index];
        HardwareData subtypeScriptableObject = ScriptableObject.CreateInstance(hardwareSubtype.ToString()) as HardwareData;
        activeHardware[index].AssignSubtypeData(subtypeScriptableObject);
    }

    void GeneratePassiveHardwareComponent(HardwareType newHardwareType, int index)
    {
        if (passiveHardware[index] != null)
        {
            Debug.LogError("Already passive hardware in that slot");
            return;
        }
        Type newHardware = GetHardwareType(newHardwareType);
        IHardware newPassiveHardware = gameObject.AddComponent(newHardware) as IHardware;
        passiveHardwareDelegates[index] += newPassiveHardware.ApplyPassiveHardware;

        passiveHardware[index] = newPassiveHardware;
    }

    void GenerateRenewable(RenewableTypes newRenewableType)
    {
        if (newRenewableType == RenewableTypes.None)
        {
            return;
        }
        Type newRenewable = GetRenewableType(newRenewableType);
        equippedRenewable = gameObject.AddComponent(newRenewable) as IRenewable;
    }

    void ClearActiveHardware(int index)
    {
        if (index == 0 || index == 1)
        {
            Debug.LogError("Trying to unequip Blink or Parry.");
            return;
        }
        IHardware equippedHardware = activeHardware[index];
        Component hardwareComponent = GetComponent(equippedHardware.GetType());
        Destroy(hardwareComponent);
        activeHardware[index] = null;
    }

    void ClearPassiveHardware(int index)
    {
        IHardware equippedHardware = passiveHardware[index];
        passiveHardwareDelegates[index] -= equippedHardware.ApplyPassiveHardware;

        Component hardwareComponent = GetComponent(equippedHardware.GetType());
        Destroy(hardwareComponent);
        passiveHardware[index] = null;
    }

    void ClearRenewable()
    {
        Component renewableComponent = GetComponent(equippedRenewable.GetType());
        Destroy(renewableComponent);
        renewableComponent = null;
    }

    public void ApplyPassiveHardware(Type newHardware, GameObject subject)
    {
        int hardwareIndex = -1;
        IHardware hardware = activeHardware[0];
        // Basically "find hardware of type"
        for (int i = 0; i < activeHardware.Length; i++)
        {
            hardware = activeHardware[i];
            if (hardware != null && hardware.GetType() == newHardware)
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
