using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
public class PositionResetTrigger : MonoBehaviour {

    [SerializeField]
    Transform resetPosition;
    [SerializeField]
    ScrollingAlertTextController scrollingAlertTextController;

    string firstScrollPrimaryText = "Yoops";
    string firstScrollSecondaryText = "If you go down there you'll be left to wander forever.";
    string secondScrollPrimaryText = "Let's Just Put You Back";
    string secondScrollSecondaryText = "Make like it never happened.";

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<BlinkHardware>().BlinkToPosition(resetPosition.position);
        scrollingAlertTextController.ForceEnqueueScrollAction(firstScrollPrimaryText, firstScrollSecondaryText);
        scrollingAlertTextController.ForceEnqueueScrollAction(secondScrollPrimaryText, secondScrollSecondaryText);
    }
}
