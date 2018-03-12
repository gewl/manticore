using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractableObjectController {

    public void RegisterInteraction()
    {
        GameManager.MenuManager.ToggleInventoryMenu();
    }
}
