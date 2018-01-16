using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractableObjectController {

    [SerializeField]
    MenuManager menuManager;

    public void RegisterInteraction()
    {
        menuManager.ToggleInventoryMenu();
    }

}
