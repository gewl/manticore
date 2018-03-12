using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationalPartnerController : MonoBehaviour, IInteractableObjectController {

    public void RegisterInteraction()
    {
        GameManager.MenuManager.ToggleDialogueMenu();
    }
}
