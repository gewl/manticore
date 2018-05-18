using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

    [SerializeField]
    string newText;
    [SerializeField]
    TutorialDoor doorController;

    public GameObject secondaryBubSet;

    public bool IsEquipActiveRoom = false;
    public bool IsEquipPassiveRoom = false;

    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        tutorialController.ChangeTutorialBub(newText, secondaryBubSet);
        doorController.CloseDoor();

        if (IsEquipActiveRoom)
        {
            tutorialController.InEquipActiveRoom = true;
        }
        else if (IsEquipPassiveRoom)
        {
            tutorialController.InEquipPassiveRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsEquipActiveRoom)
        {
            tutorialController.InEquipActiveRoom = false;
        } 
        else if (IsEquipPassiveRoom)
        {
            tutorialController.InEquipPassiveRoom = false;
        }
    }
}
