using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

    [SerializeField]
    string newText;
    [SerializeField]
    TutorialDoor doorController;

    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        tutorialController.ChangeTutorialBub(newText);
        doorController.CloseDoor();
    }
}
