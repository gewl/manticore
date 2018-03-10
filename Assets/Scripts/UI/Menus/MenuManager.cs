using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    static MenuManager instance;

    bool isInMenu = false;

    [SerializeField]
    InventoryMenuController inventoryMenuController;
    [SerializeField]
    TooltipController tooltip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ToggleInventoryMenu()
    {
        isInMenu = !isInMenu;
        inventoryMenuController.gameObject.SetActive(isInMenu);

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
