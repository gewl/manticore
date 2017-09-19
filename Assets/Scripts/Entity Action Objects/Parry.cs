using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        BasicBullet bullet = other.GetComponent<BasicBullet>();
        bullet.Parry(transform);
    }
}
