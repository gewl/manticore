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
            Transform slot = transform.GetChild(i);

            Transform activeHardware = slot.GetChild(0);
            EventTrigger activeHardwareSlotEventTrigger = activeHardware.GetComponent<EventTrigger>();
            AssignDropEventHandler(activeHardwareSlotEventTrigger, i);
            AssignClickEventHandler(activeHardwareSlotEventTrigger, i);
            AssignPointerEnterExitEventHandlers(activeHardwareSlotEventTrigger, i);

            activeHardwareImages[i] = activeHardware.GetComponent<Image>();

            Transform mods = slot.GetChild(1);
            Transform passiveHardware = mods.GetChild(0);
            EventTrigger passiveHardwareSlotEventTrigger = passiveHardware.GetComponent<EventTrigger>();
            AssignDropEventHandler(passiveHardwareSlotEventTrigger, i, false);
            AssignClickEventHandler(passiveHardwareSlotEventTrigger, i, false);
            AssignPointerEnterExitEventHandlers(passiveHardwareSlotEventTrigger, i, false);

            passiveHardwareImages[i] = passiveHardware.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        UpdateEquippedGear(InventoryController.Inventory);

        InventoryController.OnInventoryUpdated += UpdateEquippedGear;
        inventoryMenuController.OnDraggingHardware += FlagInvalidDropBubs;
        inventoryMenuController.OnStopDraggingHardware += UnflagInvalidDropBubs;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= UpdateEquippedGear;
        inventoryMenuController.OnDraggingHardware -= FlagInvalidDropBubs;
        inventoryMenuController.OnStopDraggingHardware -= UnflagInvalidDropBubs;
    }

    void FlagInvalidDropBubs(HardwareType hardwareType)
    {
        HardwareType[] activeHardware = InventoryController.Inventory.EquippedActiveHardware;
        for (int i = 0; i < activeHardware.Length; i++)
        {
            HardwareType thisHardware = activeHardware[i];

            if (thisHardware == HardwareType.None)
            {
                passiveHardwareImages[i].color = Color.blue;
            }
        }
    }

    void UnflagInvalidDropBubs(HardwareType hardwareType)
    {
        for (int i = 0; i < passiveHardwareImages.Length; i++)
        {
            Image passiveHardwareImage = passiveHardwareImages[i];
            passiveHardwareImage.color = Color.white; 
        }
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

    void AssignPointerEnterExitEventHandlers(EventTrigger trigger, int slot, bool isActiveHardware = true)
    {
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter,
        };
        pointerEnterEntry.callback.AddListener(GenerateInventoryButtonListener_PointerEnter(slot, isActiveHardware));

        trigger.triggers.Add(pointerEnterEntry);
        
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit,
        };
        pointerExitEntry.callback.AddListener(GenerateInventoryButtonListener_PointerExit(slot, isActiveHardware));

        trigger.triggers.Add(pointerExitEntry);
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
            if (isActiveHardware && (slot == 0 || slot == 1))
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

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerEnter(int slot, bool isActiveHardware)
    {
        return (data) =>
        {
            if (isActiveHardware)
            {
                inventoryMenuController.GenerateActiveHardwareTooltip(slot);
            }
            else
            {
                inventoryMenuController.GeneratePassiveHardwareTooltip(slot);
            }
        };
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener_PointerExit(int slot, bool isActiveHardware)
    {
        return (data) =>
        {
            inventoryMenuController.DeactivateTooltip();
        };
    }

    void UpdateEquippedGear(InventoryData inventory)
    {
        for (int i = 0; i < inventory.EquippedActiveHardware.Length; i++)
        {
            HardwareType equippedHardware = inventory.EquippedActiveHardware[i];
            activeHardwareImages[i].sprite = DataAssociations.GetHardwareTypeBubImage(equippedHardware);
        }

        for (int i = 0; i < inventory.EquippedPassiveHardware.Length; i++)
        {
            HardwareType equippedHardware = inventory.EquippedPassiveHardware[i];
            passiveHardwareImages[i].sprite = DataAssociations.GetHardwareTypeBubImage(equippedHardware);
        }
    }
}
