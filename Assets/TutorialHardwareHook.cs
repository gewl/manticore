using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHardwareHook : MonoBehaviour {

    [SerializeField]
    GameObject tutorialPane;
    [SerializeField]
    string newBubText;

    bool isApplicationQuitting = false;
    TutorialController tutorialController;

    private void Awake()
    {
        tutorialController = GameObject.FindGameObjectWithTag("TutorialController").GetComponent<TutorialController>();
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true; 
    }
    private void OnDestroy()
    {
        if (!isApplicationQuitting)
        {
            tutorialController.ChangeTutorialBub(newBubText, tutorialPane);
        }
    }
}
