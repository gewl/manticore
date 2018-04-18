using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class RenewableInventoryMenuController : MonoBehaviour {

    Image[] renewableInventoryImages;
    EventTrigger[] renewableInventoryEventTriggers;
    RenewableTypes[] renewableTypes;

    InventoryMenuController inventoryMenuController;

    private void Awake()
    {
        inventoryMenuController = GetComponentInParent<InventoryMenuController>();

        renewableInventoryImages = GetComponentsInChildren<Image>();
        renewableInventoryEventTriggers = GetComponentsInChildren<EventTrigger>();

        RenewableTypes[] allRenewableTypes = (RenewableTypes[])Enum.GetValues(typeof(RenewableTypes));
        renewableTypes = allRenewableTypes.Skip(1).ToArray();
    }

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += UpdateAvailability;

        DisplayAndActivateDiscoveredRenewables();

        UpdateAvailability(InventoryController.Inventory);
    }

    void DisplayAndActivateDiscoveredRenewables()
    {
        for (int i = 0; i < renewableTypes.Length; i++)
        {
            RenewableTypes renewableType = renewableTypes[i];
            if (InventoryController.HasDiscoveredRenewable(renewableType))
            {
                Sprite discoverableRenewableBubImage = DataAssociations.GetRenewableTypeBubImage(renewableType);
                renewableInventoryImages[i].sprite = discoverableRenewableBubImage;

                EventTrigger trigger = renewableInventoryEventTriggers[i];

                AssignDragEventListeners(trigger, discoverableRenewableBubImage, renewableType);
            }
        }
    }

    void AssignDragEventListeners(EventTrigger trigger, Sprite bubImage, RenewableTypes renewableType)
    {
        // Begin drag listener
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.BeginDrag,
        };
        beginDragEntry.callback.AddListener(GenerateInventoryButtonListener_BeginDrag(bubImage, renewableType));

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
        pointerEnterEntry.callback.AddListener(GenerateInventoryButtonListener_PointerEnter(renewableType));

        trigger.triggers.Add(pointerEnterEntry);

        // Point exit listener
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit,
        };
        pointerExitEntry.callback.AddListener(GenerateInventoryButtonListener_PointerExit());

        trigger.triggers.Add(pointerExitEntry);
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_BeginDrag(Sprite image, RenewableTypes renewableType)
    {
        return (data) =>
        {
            if (InventoryController.Inventory.EquippedRenewable == renewableType)
            {
                return;
            }
            inventoryMenuController.BeginDragging(image, renewableType);
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_EndDrag()
    {
        return (data) =>
        {
            inventoryMenuController.EndDrag();
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerEnter(RenewableTypes renewableType)
    {
        return (data) =>
        {
            inventoryMenuController.RenewableInventoryMenu_PointerEnter(renewableType);
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
        for (int i = 0; i < renewableTypes.Length; i++)
        {
            RenewableTypes renewableType = renewableTypes[i];

            if (inventory.EquippedRenewable == renewableType)
            {
                renewableInventoryImages[i].color = Color.grey;
            }
            else
            {
                renewableInventoryImages[i].color = Color.white;
            }
        }
    }

}
