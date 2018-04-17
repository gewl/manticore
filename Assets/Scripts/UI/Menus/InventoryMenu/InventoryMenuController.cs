using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour {

    public delegate void MenuEventListener(HardwareType hardwareType);
    public MenuEventListener OnDraggingElement;
    public MenuEventListener OnStopDraggingElement;

    MenuManager menuManager;

    HardwareType draggingHardwareType;
    Type draggingHardwareSubtype;

    [SerializeField]
    Image draggingImage;

    private void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();
    }

    private void OnDisable()
    {
        DeactivateTooltip();
    }

    // TODO: Overload this for other gearTypes, or cast enum value to int,
    // then cast int back to whichever enum is being dragged?
    public void BeginDragging(Sprite image, HardwareType hardwareType, Type hardwareSubtype)
    {
        menuManager.DepopulateInformationText();

        draggingHardwareType = hardwareType;
        draggingHardwareSubtype = hardwareSubtype;

        draggingImage.sprite = image;
        draggingImage.preserveAspect = true;
        draggingImage.gameObject.SetActive(true);

        OnDraggingElement(draggingHardwareType);
    }

    public void EndDrag()
    {
        // TODO: This will need to be fixed to work with other draggable inventory items.
        if (draggingHardwareType == HardwareType.None)
        {
            return;
        }

        draggingImage.gameObject.SetActive(false);
        OnStopDraggingElement(draggingHardwareType);

        draggingHardwareType = HardwareType.None;
        draggingHardwareSubtype = null;
    }

    public void HardwareInventoryMenu_PointerEnter(HardwareType hardwareType)
    {
        string hardwareDescription = MasterSerializer.GetGeneralHardwareDescription(hardwareType);
        menuManager.PopulateInformationText(hardwareType.ToString(), hardwareDescription);
    }

    public void DeactivateTooltip()
    {
        menuManager.DepopulateInformationText();
    }

    public void GenerateActiveHardwareTooltip(int activeHardwareSlot)
    {
        if (draggingHardwareType != HardwareType.None && (activeHardwareSlot == 0 || activeHardwareSlot == 1))
        {
            return;
        }
        HardwareType hardwareType = draggingHardwareType != HardwareType.None ? draggingHardwareType : InventoryController.GetEquippedActiveHardware()[activeHardwareSlot];
        if (hardwareType == HardwareType.None)
        {
            return;
        }

        string hardwareDescription = MasterSerializer.GetSpecificHardwareDescription(hardwareType, hardwareType);
        menuManager.PopulateInformationText(hardwareType.ToString(), hardwareDescription);
    }

    public void GeneratePassiveHardwareTooltip(int passiveHardwareSlot)
    {
        HardwareType hardwareType = draggingHardwareType != HardwareType.None ? draggingHardwareType : InventoryController.GetEquippedPassiveHardware()[passiveHardwareSlot];
        HardwareType activeHardwareType = InventoryController.GetEquippedActiveHardware()[passiveHardwareSlot];

        if (hardwareType == HardwareType.None || activeHardwareType == HardwareType.None)
        {
            return;
        }

        string hardwareDescription = MasterSerializer.GetSpecificHardwareDescription(hardwareType, activeHardwareType);
        menuManager.PopulateInformationText(hardwareType.ToString(), hardwareDescription);
    }

    public void EquipDraggedActiveHardware(int slot)
    {
        if (draggingHardwareType == HardwareType.None)
        {
            return;
        }
        InventoryController.EquipActiveHardware(slot, draggingHardwareType, draggingHardwareSubtype);
        EndDrag();
    }

    public void EquipDraggedPassiveHardware(int slot)
    {
        if (draggingHardwareType == HardwareType.None)
        {
            return;
        }
        InventoryController.EquipPassiveHardware(slot, draggingHardwareType);
        EndDrag();
    }
}
