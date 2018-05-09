using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManipulator : MonoBehaviour, IInteractableObjectController {

    [SerializeField]
    StatefulObject attachedObject;

    public void RegisterInteraction()
    {
        attachedObject.Move();
    }
}
