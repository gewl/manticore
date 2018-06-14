using UnityEngine;

public class EquipPassiveBubSequencer : MonoBehaviour
{
    [SerializeField]
    GameObject dragBub;
    [SerializeField]
    GameObject onParryBub;
    [SerializeField]
    InventoryMenuController inventoryMenu;

    private void OnEnable()
    {
        InventoryController.OnInventoryUpdated += DragBubActivator;
    }

    private void OnDisable()
    {
        InventoryController.OnInventoryUpdated -= DragBubActivator;
        if (inventoryMenu.OnDraggingHardware != null)
        {
            inventoryMenu.OnDraggingHardware -= OnParryBubActivator;
        }
    }

    void DragBubActivator(InventoryData inventory)
    {
        if (inventory.EquippedActiveHardware[2] == HardwareType.None)
        {
            dragBub.SetActive(true);
            InventoryController.OnInventoryUpdated -= DragBubActivator;
            inventoryMenu.OnDraggingHardware += OnParryBubActivator;
        }
    }

    void OnParryBubActivator(HardwareType hardwareType)
    {
        if (hardwareType == HardwareType.Nullify)
        {
            onParryBub.SetActive(true);
            inventoryMenu.OnDraggingHardware -= OnParryBubActivator;
        }
    }
}