using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour {

    LayerMask enemyBulletLayer;

    private void Awake()
    {
        enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyBulletLayer)
        {

        }        
    }
}
