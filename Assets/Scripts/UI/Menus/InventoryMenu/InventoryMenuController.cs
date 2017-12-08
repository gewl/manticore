using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuController : MonoBehaviour {

    bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }

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
}
