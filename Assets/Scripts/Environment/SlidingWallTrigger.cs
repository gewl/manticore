using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingWallTrigger : MonoBehaviour {

    RoomController roomController;

    void Awake()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    void OnTriggerEnter(Collider other)
    {
        roomController.RegisterEnterTrigger();
    }

    void OnTriggerExit(Collider other)
    {
        roomController.RegisterExitTrigger();
    }
}