using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour {

    public delegate void MenuEventListener(HardwareTypes hardwareType);
    public MenuEventListener OnDraggingElement;
    public MenuEventListener OnStopDraggingElement;

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

    // TODO: Overload this for other gearTypes, or cast enum value to int,
    // then cast int back to whichever enum is being dragged?
    public void BeginDragging(Sprite image, HardwareTypes hardwareType)
    {
        draggingGearType = GearTypes.Hardware;
        draggingHardwareType = hardwareType;

        draggingImage.sprite = image;
        draggingImage.preserveAspect = true;
        draggingImage.gameObject.SetActive(true);

        OnDraggingElement(draggingHardwareType);
    }

    public void DragUpdate()
    {
        draggingImage.rectTransform.position = Input.mousePosition;
    }
    
    public void EndDrag()
    {
        draggingImage.gameObject.SetActive(false);
        OnStopDraggingElement(draggingHardwareType);
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
