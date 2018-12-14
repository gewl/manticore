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
            // TODO: This was causing issues when an entity was killed before player exited,
            // and wasn't being used for much anyway. Fix it up in the future.
            //areaController.RegisterEntityEnter(other.gameObject, floor);
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
            //areaController.RegisterEntityExit(other.gameObject, floor);
        }
    }
}