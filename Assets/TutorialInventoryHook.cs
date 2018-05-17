using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInventoryHook : MonoBehaviour {

    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnEnable()
    {
        tutorialController.RegisterInventoryMenuOpen();
    }

    private void OnDisable()
    {
        tutorialController.RegisterInventoryMenuClose();
    }
}
