using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGearManagement : MonoBehaviour {

    IHardware[] activeHardware;
    List<IHardware> passiveHardware;
    ApplyPassiveHardwareDelegate[] passiveHardwareDelegates;

    public IHardware ParryGear { get { return activeHardware[0]; } }
    public IHardware BlinkGear { get { return activeHardware[1]; } }
    public IHardware EquippedGear_Slot2 { get { return activeHardware[2]; } }
    public IHardware EquippedGear_Slot3 { get { return activeHardware[3]; } }

    delegate void ApplyPassiveHardwareDelegate(HardwareTypes activeHardwareType, IHardware activeHardware, GameObject subject);
    ApplyPassiveHardwareDelegate passiveHardware_Parry;
    ApplyPassiveHardwareDelegate passiveHardware_Blink;
    ApplyPassiveHardwareDelegate passiveHardware_Slot2;
    ApplyPassiveHardwareDelegate passiveHardware_Slot3;

    public delegate void PassActiveHardware(ref IHardware[] activeHardware);
    public PassActiveHardware activeHardwareUpdated;

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
        for (int i = 2; i < inventory.activeHardware.Length; i++)
        {
            HardwareTypes hardwareType = inventory.activeHardware[i];
            GenerateActiveHardwareComponent(hardwareType, i);
        }

        for (int i = 0; i < inventory.passiveHardware.Length; i++)
        {
            HardwareTypes hardwareType = inventory.passiveHardware[i];
            GeneratePassiveHardwareComponent(hardwareType, i);
        }

        if (activeHardwareUpdated != null)
        {
            activeHardwareUpdated(ref activeHardware);
        }
    }

    Type GetHardwareType(HardwareTypes hardwareType)
    {
        switch (hardwareType)
        {
            case HardwareTypes.None:
                return null;
            case HardwareTypes.Parry:
                return typeof(ParryHardware);
            case HardwareTypes.Blink:
                return typeof(BlinkHardware);
            case HardwareTypes.Nullify:
                return typeof(NullifierHardware);
            case HardwareTypes.Riposte:
                return typeof(RiposteHardware);
            default:
                return null;
        }
    }
    
    void GenerateActiveHardwareComponent(HardwareTypes newHardwareType, int index)
    {
        if (newHardwareType == HardwareTypes.None)
        {
            return;
        }
        Type newHardware = GetHardwareType(newHardwareType);
        activeHardware[index] = gameObject.AddComponent(newHardware) as IHardware;
    }

    void GeneratePassiveHardwareComponent(HardwareTypes newHardwareType, int index)
    {
        if (newHardwareType == HardwareTypes.None)
        {
            return;
        }
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
