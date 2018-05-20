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
    public bool IsRenewableRoom = false;

    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        tutorialController.ChangeTutorialBub(newText, secondaryBubSet);
        if (doorController != null)
        {
            doorController.CloseDoor();
        }

        if (IsEquipActiveRoom)
        {
            tutorialController.InEquipActiveRoom = true;
        }
        else if (IsEquipPassiveRoom)
        {
            tutorialController.InEquipPassiveRoom = true;
        }
        else if (IsRenewableRoom)
        {
            GameManager.GetPlayerTransform().GetComponent<MobileEntityHealthComponent>().ReceiveDamageDirectly(50f);
            tutorialController.InRenewableRoom = true;
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
        else if (IsRenewableRoom)
        {
            tutorialController.InRenewableRoom = false;
        }
    }
}
