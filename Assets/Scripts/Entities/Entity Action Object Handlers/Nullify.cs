using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nullify : MonoBehaviour {

    public bool IsReflecting = false;
    float handicap = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (IsReflecting)
        {
            BulletController bullet = other.GetComponent<BulletController>();

            if (bullet != null && bullet.CompareTag(BulletController.ENEMY_BULLET))
            {
                bullet.Parry(transform.parent, bullet.strength * handicap, handicap);
            }
        } 
    }

}
