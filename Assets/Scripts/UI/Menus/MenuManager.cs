using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    bool isInMenu = false;
    GameObject currentlyActiveMenu;

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

    void EnterMenu(GameObject menu)
    {
        isInMenu = true;
        GameManager.EnterMenu();
        menu.SetActive(true);
        currentlyActiveMenu = menu;
    }

    void LeaveMenu()
    {
        isInMenu = false;
        currentlyActiveMenu.SetActive(false);
        GameManager.ExitMenu();
        currentlyActiveMenu = null;
    }

    public void ToggleDialogueMenu(string conversationalPartnerID = "")
    {
        if (!isInMenu)
        {
            EnterMenu(DialogueMenu.gameObject);
            DialogueMenu.RegisterConversationalPartner(conversationalPartnerID);
        }
        else
        {
            LeaveMenu();
        }
    }

    public void TransitionToNonDialogueMenu(GlobalConstants.Menus newMenu)
    {
        DialogueMenu.gameObject.SetActive(false);
        GameObject newMenuObject = DialogueMenu.gameObject;
        switch (newMenu)
        {
            case GlobalConstants.Menus.Dialogue:
                Debug.LogError("Trying to transition to Dialogue menu from Dialogue menu.");
                break;
            case GlobalConstants.Menus.Inventory:
                newMenuObject = InventoryMenu.gameObject;
                break;
            default:
                break;
        }

        if (newMenuObject != null)
        {
            newMenuObject.SetActive(true);
            currentlyActiveMenu = newMenuObject;
        }
    }

    public void ToggleInventoryMenu()
    {
        if (!isInMenu)
        {
            EnterMenu(InventoryMenu.gameObject);
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
