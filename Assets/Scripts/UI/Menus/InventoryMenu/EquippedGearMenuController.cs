using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EquippedGearMenuController : MonoBehaviour {

    InventoryMenuController inventoryMenuController;

    Image[] activeHardwareImages;
    Image[] passiveHardwareImages;

    private void Awake()
    {
        inventoryMenuController = GetComponentInParent<InventoryMenuController>();

        int childCount = transform.childCount;

        activeHardwareImages = new Image[4];
        passiveHardwareImages = new Image[4];

        // Populate arrays of images, assign event handlers for drop
        for (int i = 0; i < childCount; i++)
        {
            Transform activeHardware = transform.GetChild(i);
            EventTrigger activeHardwareSlotEventTrigger = activeHardware.GetComponent<EventTrigger>();
            AssignDropEventHandler(activeHardwareSlotEventTrigger, i);
            AssignClickEventHandler(activeHardwareSlotEventTrigger, i);

            activeHardwareImages[i] = activeHardware.GetComponent<Image>();

            Transform passiveHardware = activeHardware.GetChild(0);
            EventTrigger passiveHardwareSlotEventTrigger = passiveHardware.GetComponent<EventTrigger>();
            AssignDropEventHandler(passiveHardwareSlotEventTrigger, i, false);
            AssignClickEventHandler(passiveHardwareSlotEventTrigger, i, false);

            passiveHardwareImages[i] = passiveHardware.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        UpdateEquippedGear(InventoryController.Inventory);

        InventoryController.OnInventoryUpdated += UpdateEquippedGear;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= UpdateEquippedGear;
    }

    // This is a little clumsy: using isActiveHardware bool to differentiate between Active and Passive,
    // for purpose of event handler notifying the InventoryController.
    void AssignDropEventHandler(EventTrigger trigger, int slot, bool isActiveHardware = true)
    {
        EventTrigger.Entry dropEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Drop,
        };
        dropEntry.callback.AddListener(GenerateInventoryButtonListener_Drop(slot, isActiveHardware));

        trigger.triggers.Add(dropEntry);
    }

    void AssignClickEventHandler(EventTrigger trigger, int slot, bool isActiveHardware = true)
    {
        EventTrigger.Entry clickEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick,
        };
        clickEntry.callback.AddListener(GenerateInventoryButtonListener_Click(slot, isActiveHardware));

        trigger.triggers.Add(clickEntry);
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_Drop(int slot, bool isActiveHardware)
    {
        return (data) =>
        {
            if (isActiveHardware)
            {
                inventoryMenuController.EquipDraggedActiveHardware(slot);
            }
            else
            {
                inventoryMenuController.EquipDraggedPassiveHardware(slot);
            }
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_Click(int slot, bool isActiveHardware)
    {
        return (data) =>
        {
            if (isActiveHardware && slot == 0 || slot == 1)
            {
                return;
            }
            if (isActiveHardware)
            {
                InventoryController.UnequipActiveHardware(slot);
            }
            else
            {
                InventoryController.UnequipPassiveHardware(slot);
            }
        };
    }

    void UpdateEquippedGear(InventoryData inventory)
    {
        for (int i = 0; i < inventory.activeHardware.Length; i++)
        {
            HardwareTypes equippedHardware = inventory.activeHardware[i];
            activeHardwareImages[i].sprite = DataAssociations.GetHardwareTypeBubImage(equippedHardware);
        }

        for (int i = 0; i < inventory.passiveHardware.Length; i++)
        {
            HardwareTypes equippedHardware = inventory.passiveHardware[i];
            passiveHardwareImages[i].sprite = DataAssociations.GetHardwareTypeBubImage(equippedHardware);
        }
    }
}
