using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

    [SerializeField]
    string newText;

    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        tutorialController.ChangeTutorialBub(newText);
    }
}
