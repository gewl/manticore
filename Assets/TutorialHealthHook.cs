using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHealthHook : EntityComponent {

    TutorialController tutorialController;
    public string newBubText;
    public GameObject secondaryBubPane;

    protected override void Awake()
    {
        base.Awake();

        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.HealthChanged, OnHealthChanged);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.HealthChanged, OnHealthChanged);
    }

    void OnHealthChanged()
    {
        if (tutorialController.InRenewableRoom)
        {
            tutorialController.ChangeTutorialBub(newBubText, secondaryBubPane);
        }
    }
}
