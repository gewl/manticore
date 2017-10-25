using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWallTrigger : MonoBehaviour {

    RoomController roomController;
    bool isPlayerInside = false;

    void Awake()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isPlayerInside)
        {
            roomController.RegisterEnterTrigger();
            isPlayerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isPlayerInside)
        {
            roomController.RegisterExitTrigger();
            isPlayerInside = false;
        }
    }
}