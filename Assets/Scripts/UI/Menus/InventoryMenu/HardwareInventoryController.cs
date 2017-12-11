using System;    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HardwareInventoryController : MonoBehaviour {

    Image[] hardwareInventoryImages;
    EventTrigger[] hardwareInventoryEventTriggers;
    HardwareTypes[] discoverableHardwareTypes;

    private void Awake()
    {
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
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.BeginDrag,
                };
                entry.callback.AddListener(GenerateInventoryButtonListener(hardwareType));

                trigger.triggers.Add(entry);
            }
        }
    }

    UnityAction<BaseEventData> GenerateInventoryButtonListener(HardwareTypes hardwareType)
    {
        return (data) =>
        {
            Debug.Log(hardwareType);
        };
    }

    void UpdateAvailability(InventoryData inventory)
    {
        for (int i = 0; i < discoverableHardwareTypes.Length; i++)
        {
            HardwareTypes hardwareType = discoverableHardwareTypes[i];

            if (inventory.activeHardware.Contains(hardwareType))
            {
                hardwareInventoryImages[i].color = Color.grey;
                hardwareInventoryButtons[i].interactable = false;
            }
            else
            {
                hardwareInventoryImages[i].color = Color.white;
                hardwareInventoryButtons[i].interactable = true;
            }
        }
    }
}
