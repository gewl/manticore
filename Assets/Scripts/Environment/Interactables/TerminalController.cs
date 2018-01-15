using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractableObjectController {

    [SerializeField]
    InventoryMenuController inventoryMenu;

    public void RegisterInteraction()
    {
        inventoryMenu.ToggleMenu();
    }

}
