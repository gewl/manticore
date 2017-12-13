using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour {

    bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

    GearTypes draggingGearType;
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

    public void BeginDragging(GearTypes gearType, Sprite image)
    {
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

    public void OnInventoryPress()
    {
        Debug.Log("pressed");
    }

}
