using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuController : MonoBehaviour {

    public delegate void HardwareMenuEventListener(HardwareType hardwareType);
    public HardwareMenuEventListener OnDraggingHardware;
    public HardwareMenuEventListener OnStopDraggingHardware;

    public delegate void RenewableMenuEventListener(RenewableTypes renewableType);
    public RenewableMenuEventListener OnDraggingRenewable;
    public RenewableMenuEventListener OnStopDraggingRenewable;

    MenuManager menuManager;

    GearTypes currentDraggingType;
    HardwareType draggingHardwareType;
    Type draggingHardwareSubtype;

    RenewableTypes draggingRenewableType;

    [SerializeField]
    Image draggingImage;

    private void Awake()
    {
        menuManager = GetComponentInParent<MenuManager>();
    }

    private void OnDisable()
    {
        DeactivateTooltip();
        InventoryController.OnInventoryUpdated(InventoryController.Inventory);
    }

    // TODO: Overload this for other gearTypes, or cast enum value to int,
    // then cast int back to whichever enum is being dragged?
    public void BeginDragging(Sprite image, HardwareType hardwareType, Type hardwareSubtype)
    {
        menuManager.DepopulateInformationText();

        currentDraggingType = GearTypes.Hardware;
        draggingHardwareType = hardwareType;
        draggingHardwareSubtype = hardwareSubtype;

        draggingImage.sprite = image;
        draggingImage.preserveAspect = true;
        draggingImage.gameObject.SetActive(true);

        OnDraggingHardware(draggingHardwareType);
    }

    public void BeginDragging(Sprite image, RenewableTypes renewableType)
    {
        menuManager.DepopulateInformationText();

        currentDraggingType = GearTypes.Renewable;
        draggingRenewableType = renewableType;

        draggingImage.sprite = image;
        draggingImage.preserveAspect = true;
        draggingImage.gameObject.SetActive(true);

        OnDraggingHardware(draggingHardwareType);
    }

    public void EndDrag()
    {
        if (currentDraggingType == GearTypes.Hardware)
        {
            if (draggingHardwareType == HardwareType.None)
            {
                return;
            }

            draggingImage.gameObject.SetActive(false);
            if (OnStopDraggingHardware != null)
            {
                OnStopDraggingHardware(draggingHardwareType);
            }

            draggingHardwareType = HardwareType.None;
            draggingHardwareSubtype = null;
        }
        else if (currentDraggingType == GearTypes.Renewable)
        {
            if (draggingRenewableType == RenewableTypes.None)
            {
                return;
            }

            draggingImage.gameObject.SetActive(false);
            if (OnStopDraggingRenewable != null)
            {
                OnStopDraggingRenewable(draggingRenewableType);
            }

            draggingRenewableType = RenewableTypes.None;
        }
    }

    public void HardwareInventoryMenu_PointerEnter(HardwareType hardwareType)
    {
        string hardwareDescription = MasterSerializer.GetGeneralHardwareDescription(hardwareType);
        menuManager.PopulateInformationText(hardwareType.ToString(), hardwareDescription);
    }

    public void RenewableInventoryMenu_PointerEnter(RenewableTypes renewableType)
    {
        string renewableDescription = MasterSerializer.GetRenewableDescription(renewableType);
        menuManager.PopulateInformationText(renewableType.ToString(), renewableDescription);
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
        if (currentDraggingType != GearTypes.Hardware || draggingHardwareType == HardwareType.None)
        {
            return;
        }
        InventoryController.EquipActiveHardware(slot, draggingHardwareType, draggingHardwareSubtype);
        EndDrag();
    }

    public void EquipDraggedPassiveHardware(int slot)
    {
        if (currentDraggingType != GearTypes.Hardware || draggingHardwareType == HardwareType.None)
        {
            return;
        }
        InventoryController.EquipPassiveHardware(slot, draggingHardwareType);
        EndDrag();
    }

    public void EquipDraggedRenewable()
    {
        if (currentDraggingType != GearTypes.Renewable || draggingRenewableType == RenewableTypes.None)
        {
            return;
        }

        InventoryController.EquipRenewable(draggingRenewableType);
        EndDrag();
    }
}
