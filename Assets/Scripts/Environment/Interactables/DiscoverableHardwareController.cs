using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverableHardwareController : MonoBehaviour, IInteractableObjectController {

    [SerializeField]
    HardwareType typeToDiscover;

    public void RegisterInteraction()
    {
        InventoryController.DiscoverHardware(typeToDiscover);

        Destroy(gameObject);
    }

}
