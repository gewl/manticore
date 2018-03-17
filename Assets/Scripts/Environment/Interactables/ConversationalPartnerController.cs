using UnityEngine;

public class ConversationalPartnerController : MonoBehaviour, IInteractableObjectController {

    public void RegisterInteraction()
    {
        GameManager.MenuManager.ToggleDialogueMenu(gameObject.name);
    }
}
