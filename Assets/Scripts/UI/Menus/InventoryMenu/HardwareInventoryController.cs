using System;    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HardwareInventoryController : MonoBehaviour {

    Image[] hardwareInventoryImages;
    HardwareTypes[] discoverableHardwareTypes;

    private void Awake()
    {
        hardwareInventoryImages = GetComponentsInChildren<Image>();
        HardwareTypes[] allHardwareTypes = (HardwareTypes[])Enum.GetValues(typeof(HardwareTypes));
        discoverableHardwareTypes = allHardwareTypes.Skip(3).ToArray();
    }

    private void OnEnable()
    {
        for (int i = 0; i < discoverableHardwareTypes.Length; i++)
        {
            HardwareTypes hardwareType = discoverableHardwareTypes[i];
            if (InventoryController.HasDiscoveredHardware(hardwareType))
            {
                Sprite discoverableHardwareBubImage = DataAssociations.GetHardwareTypeBubImage(hardwareType);
                hardwareInventoryImages[i].sprite = discoverableHardwareBubImage;
            }
        }
        //for (int i = 3; i < Enum.GetValues(typeof(HardwareTypes)).Length; i++)
        //{
        //    HardwareTypes hardwareType = (HardwareTypes)i;
        //    int buttonIndex = i - 3;
        //} 
    }
}
