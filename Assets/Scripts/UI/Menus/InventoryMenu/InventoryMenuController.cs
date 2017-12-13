using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour {

    bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

    GearTypes draggingGearType;
    HardwareTypes draggingHardwareType;

    [SerializeField]
    Image draggingImage;

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        gameObject.SetActive(isOpen);

        if (isOpen)
        {
            GameManager.EnterMenu();
        }
        else
        {
            GameManager.ExitMenu();
        }
    }

    // Overload this for other gearTypes
    public void BeginDragging(Sprite image, HardwareTypes hardwareType)
    {
        draggingGearType = GearTypes.Hardware;
        draggingHardwareType = hardwareType;

        draggingImage.sprite = image;
        draggingImage.preserveAspect = true;
        draggingImage.gameObject.SetActive(true);
    }

    public void DragUpdate()
    {
        draggingImage.rectTransform.position = Input.mousePosition;
    }
    
    public void EndDrag()
    {
        draggingImage.gameObject.SetActive(false);
    }

    public void EquipDraggedActiveHardware(int slot)
    {
        InventoryController.EquipActiveHardware(slot, draggingHardwareType);
        EndDrag();
    }

    public void EquipDraggedPassiveHardware(int slot)
    {
        InventoryController.EquipPassiveHardware(slot, draggingHardwareType);
        EndDrag();
    }
}
