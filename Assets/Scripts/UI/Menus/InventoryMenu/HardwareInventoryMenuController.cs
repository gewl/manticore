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
    HardwareTypes[] discoverableHardwareTypes;

    InventoryMenuController inventoryMenuController;

    private void Awake()
    {
        inventoryMenuController = GetComponentInParent<InventoryMenuController>();

        hardwareInventoryImages = GetComponentsInChildren<Image>();
        hardwareInventoryEventTriggers = GetComponentsInChildren<EventTrigger>();
        HardwareTypes[] allHardwareTypes = (HardwareTypes[])Enum.GetValues(typeof(HardwareTypes));
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
            HardwareTypes hardwareType = discoverableHardwareTypes[i];
            if (InventoryController.HasDiscoveredHardware(hardwareType))
            {
                Sprite discoverableHardwareBubImage = DataAssociations.GetHardwareTypeBubImage(hardwareType);
                hardwareInventoryImages[i].sprite = discoverableHardwareBubImage;

                EventTrigger trigger = hardwareInventoryEventTriggers[i];

                AssignDragEventListeners(trigger, discoverableHardwareBubImage, hardwareType);
            }
        }
    }

    void AssignDragEventListeners(EventTrigger trigger, Sprite hardwareImage, HardwareTypes hardwareType)
    {
        // Begin drag listener
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag,
        };
        beginDragEntry.callback.AddListener(GenerateInventoryButtonListener_BeginDrag(hardwareImage, hardwareType));

        trigger.triggers.Add(beginDragEntry);

        // Drag update listener
        EventTrigger.Entry dragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drag,
        };
        dragEntry.callback.AddListener(GenerateInventoryButtonListener_Drag());

        trigger.triggers.Add(dragEntry);

        // End drag listener
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.EndDrag,
        };
        endDragEntry.callback.AddListener(GenerateInventoryButtonListener_EndDrag());

        trigger.triggers.Add(endDragEntry);

    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_BeginDrag(Sprite image, HardwareTypes hardwareType)
    {
        return (data) =>
        {
            inventoryMenuController.BeginDragging(image, hardwareType);
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_Drag()
    {
        return (data) =>
        {
            inventoryMenuController.DragUpdate();
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_EndDrag()
    {
        return (data) =>
        {
            inventoryMenuController.EndDrag();
        };
    }

    void UpdateAvailability(InventoryData inventory)
    {
        for (int i = 0; i < discoverableHardwareTypes.Length; i++)
        {
            HardwareTypes hardwareType = discoverableHardwareTypes[i];

            if (inventory.activeHardware.Contains(hardwareType) || inventory.passiveHardware.Contains(hardwareType))
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
