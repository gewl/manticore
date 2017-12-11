using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour {

    bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

    bool isMouseDraggingHardware = false;
    HardwareTypes attachedHardwareType = HardwareTypes.None;

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

    public void OnInventoryPress()
    {
        Debug.Log("pressed");
    }

    public void AttachHardwareToMouse(HardwareTypes hardwareType)
    {
        isMouseDraggingHardware = true;
        attachedHardwareType = hardwareType;

    }

    IEnumerator DragHardware(HardwareTypes hardwareType)
    {
        Sprite hardwareImage = DataAssociations.GetHardwareTypeBubImage(hardwareType);

        while (true)
        {

            yield return null;
        }
    }
}
