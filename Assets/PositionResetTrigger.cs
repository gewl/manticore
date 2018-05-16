using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetTrigger : MonoBehaviour {

    [SerializeField]
    Transform resetPosition;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<BlinkHardware>().BlinkToPosition(resetPosition.position);
    }
}
