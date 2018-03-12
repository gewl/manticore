using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    bool isInMenu = false;

    DialogueMenuController _dialogueMenuController;
    DialogueMenuController DialogueMenu
    {
        get
        {
            if (_dialogueMenuController == null)
            {
                _dialogueMenuController = GetComponentInChildren<DialogueMenuController>(true);
            }

            return _dialogueMenuController;
        }
    }

    InventoryMenuController _inventoryMenuController;
    InventoryMenuController InventoryMenu
    {
        get
        {
            if (_inventoryMenuController == null)
            {
                _inventoryMenuController = GetComponentInChildren<InventoryMenuController>(true);
            }

            return _inventoryMenuController;
        }
    }
    [SerializeField]
    TooltipController tooltip;

    public void ToggleDialogueMenu()
    {
        isInMenu = !isInMenu;

        DialogueMenu.gameObject.SetActive(isInMenu);
        if (isInMenu)
        {
            GameManager.EnterMenu();
        }
        else
        {
            GameManager.ExitMenu();
        }
    }

    public void ToggleInventoryMenu()
    {
        isInMenu = !isInMenu;
        InventoryMenu.gameObject.SetActive(isInMenu);

        if (isInMenu)
        {
            GameManager.EnterMenu();
        }
        else
        {
            GameManager.ExitMenu();
        }
    }

    public void ActivateTooltip(string header, string body)
    {
        tooltip.gameObject.SetActive(true);

        tooltip.UpdateText(header, body);
    }

    public void DeactivateTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
