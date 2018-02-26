using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {

    AreaController areaController;
    bool isActive = false;

    [SerializeField]
    int floor = 1;

    int playerLayer;
    int entityLayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        entityLayer = LayerMask.NameToLayer("Entity");
        areaController = GetComponentInParent<AreaController>();
    }

    void OnTriggerEnter(Collider other)
    {
        int collidedObjectLayer = other.gameObject.layer;
        if (collidedObjectLayer == playerLayer && !isActive)
        {
            areaController.RegisterPlayerEnter(floor);
            isActive = true;
        }
        else if (collidedObjectLayer == entityLayer)
        {
            areaController.RegisterEntityEnter(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        int collidedObjectLayer = other.gameObject.layer;
        if (collidedObjectLayer == playerLayer && isActive)
        {
            areaController.RegisterPlayerExit(floor);
            isActive = false;
        }
        else if (collidedObjectLayer == entityLayer)
        {
            areaController.RegisterEntityExit(other.gameObject);
        }
    }
}