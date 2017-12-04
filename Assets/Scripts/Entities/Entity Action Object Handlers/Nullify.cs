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
            BasicBullet bullet = other.GetComponent<BasicBullet>();

            if (bullet != null && bullet.CompareTag(BasicBullet.ENEMY_BULLET))
            {
                bullet.Parry(transform.parent, bullet.strength * handicap, handicap);
            }
        } 
    }

}
