using System;    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HardwareInventoryMenuController : MonoBehaviour {

    Image[] hardwareInventoryImages;
    EventTrigger[] hardwareInventoryEventTriggers;
    HardwareType[] discoverableHardwareTypes;

    InventoryMenuController inventoryMenuController;

    private void Awake()
    {
        inventoryMenuController = GetComponentInParent<InventoryMenuController>();

        hardwareInventoryImages = GetComponentsInChildren<Image>();
        hardwareInventoryEventTriggers = GetComponentsInChildren<EventTrigger>();
        HardwareType[] allHardwareTypes = (HardwareType[])Enum.GetValues(typeof(HardwareType));
        discoverableHardwareTypes = allHardwareTypes.Skip(3).ToArray();
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += UpdateAvailability;

        DisplayAndActivateDiscoveredHardware();

        UpdateAvailability(InventoryController.Inventory);
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= UpdateAvailability;
    }

    void DisplayAndActivateDiscoveredHardware()
    {
        for (int i = 0; i < discoverableHardwareTypes.Length; i++)
        {
            HardwareType hardwareType = discoverableHardwareTypes[i];
            if (InventoryController.HasDiscoveredHardware(hardwareType))
            {
                Sprite discoverableHardwareBubImage = DataAssociations.GetHardwareTypeBubImage(hardwareType);
                hardwareInventoryImages[i].sprite = discoverableHardwareBubImage;

                EventTrigger trigger = hardwareInventoryEventTriggers[i];

                AssignDragEventListeners(trigger, discoverableHardwareBubImage, hardwareType);
            }
        }
    }

    void AssignDragEventListeners(EventTrigger trigger, Sprite hardwareImage, HardwareType hardwareType)
    {
        // Begin drag listener
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag,
        };
        beginDragEntry.callback.AddListener(GenerateInventoryButtonListener_BeginDrag(hardwareImage, hardwareType, InventoryController.GetHardwareSubtype(hardwareType)));

        trigger.triggers.Add(beginDragEntry);

        // End drag listener
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag,
        };
        endDragEntry.callback.AddListener(GenerateInventoryButtonListener_EndDrag());

        trigger.triggers.Add(endDragEntry);

        // Point enter listener
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter,
        };
        pointerEnterEntry.callback.AddListener(GenerateInventoryButtonListener_PointerEnter(hardwareType));

        trigger.triggers.Add(pointerEnterEntry);

        // Point exit listener
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit,
        };
        pointerExitEntry.callback.AddListener(GenerateInventoryButtonListener_PointerExit());

        trigger.triggers.Add(pointerExitEntry);
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_BeginDrag(Sprite image, HardwareType hardwareType, Type hardwareSubtype)
    {
        return (data) =>
        {
            inventoryMenuController.BeginDragging(image, hardwareType, hardwareSubtype);
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_EndDrag()
    {
        return (data) =>
        {
            inventoryMenuController.EndDrag();
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerEnter(HardwareType hardwareType)
    {
        return (data) =>
        {
            inventoryMenuController.HardwareInventoryMenu_PointerEnter(hardwareType);
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerExit()
    {
        return (data) =>
        {
            inventoryMenuController.DeactivateTooltip();
        };
    }

    void UpdateAvailability(InventoryData inventory)
    {
        for (int i = 0; i < discoverableHardwareTypes.Length; i++)
        {
            HardwareType hardwareType = discoverableHardwareTypes[i];

            if (inventory.EquippedActiveHardware.Contains(hardwareType) || inventory.EquippedPassiveHardware.Contains(hardwareType))
            {
                hardwareInventoryImages[i].color = Color.grey;
                hardwareInventoryEventTriggers[i].enabled = false;
            }
            else
            {
                hardwareInventoryImages[i].color = Color.white;
                hardwareInventoryEventTriggers[i].enabled = true;
            }
        }
    }
}
