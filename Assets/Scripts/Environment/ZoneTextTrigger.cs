using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTextTrigger : MonoBehaviour {

    bool hasFired = false;

    int playerLayer;
    [SerializeField]
    ScrollingAlertTextController scrollingTextController;

    [SerializeField]
    string primaryText;
    [SerializeField]
    string secondaryText;

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasFired && other.gameObject.layer == playerLayer)
        {
            hasFired = true;
            scrollingTextController.ForceEnqueueScrollAction(primaryText, secondaryText);
        }
    }

}
