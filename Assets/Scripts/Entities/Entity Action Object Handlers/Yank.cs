using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yank : MonoBehaviour {

    void Update()
    {
        Vector3 lookDirection = transform.position - GameManager.GetPlayerPosition();
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
