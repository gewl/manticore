using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test Collider fired");
    }
}
