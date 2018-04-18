using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EquippedRenewableController : MonoBehaviour {

    InventoryMenuController inventoryMenuController;

    Image activeRenewableImage;

    private void Awake()
    {
        inventoryMenuController = GetComponentInParent<InventoryMenuController>();

        activeRenewableImage = GetComponent<Image>();

        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        AssignDropEventHandler(eventTrigger);
        AssignPointerEnterExitEventHandler(eventTrigger);
    }

    private void OnEnable()
    {
        UpdateEquippedRenewable(InventoryController.Inventory);

        InventoryController.OnInventoryUpdated += UpdateEquippedRenewable;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= UpdateEquippedRenewable;
    }

    void UpdateEquippedRenewable(InventoryData inventory)
    {
        activeRenewableImage.sprite = DataAssociations.GetRenewableTypeBubImage(inventory.EquippedRenewable);
    }

    void AssignDropEventHandler(EventTrigger trigger)
    {
        EventTrigger.Entry dropEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drop,
        };
        dropEntry.callback.AddListener(GenerateInventoryButtonListener_Drop());

        trigger.triggers.Add(dropEntry);
    }

    void AssignPointerEnterExitEventHandler(EventTrigger trigger)
    {
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter,
        };
        pointerEnterEntry.callback.AddListener(GenerateInventoryButtonListener_PointerEnter());

        trigger.triggers.Add(pointerEnterEntry);
        
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit,
        };
        pointerExitEntry.callback.AddListener(GenerateInventoryButtonListener_PointerExit());

        trigger.triggers.Add(pointerExitEntry);
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_Drop()
    {
        return (data) =>
        {
            inventoryMenuController.EquipDraggedRenewable();
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerEnter()
    {
        return (data) =>
        {
            Debug.Log("pointer entered renewable");
            inventoryMenuController.RenewableInventoryMenu_PointerEnter(InventoryController.Inventory.EquippedRenewable);
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerExit()
    {
        return (data) =>
        {
            inventoryMenuController.DeactivateTooltip();
        };
    }
}