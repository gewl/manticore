using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDeathTrigger : EntityComponent {

    [SerializeField]
    string newText;
    [SerializeField]
    bool shouldSubscribeToMomentumAssignment = false;

    public GameObject secondaryBubSet;

    TutorialController tutorialController;

    protected override void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDeath);
    }

    protected override void Unsubscribe()
    {
    }

    private void OnDeath()
    {
        tutorialController.ChangeTutorialBub(newText, secondaryBubSet);
        if (shouldSubscribeToMomentumAssignment)
        {
            tutorialController.SubscribeToMomentumAssignment();
        }
    }
}
