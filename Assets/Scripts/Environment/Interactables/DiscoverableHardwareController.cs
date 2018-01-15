using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverableHardwareController : MonoBehaviour, IInteractableObjectController {

    [SerializeField]
    HardwareTypes typeToDiscover;

    public void RegisterInteraction()
    {
        InventoryController.DiscoverHardware(typeToDiscover);

        Destroy(gameObject);
    }

}
