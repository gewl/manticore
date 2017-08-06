using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHandler : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            GameObject bullet = other.gameObject;
            BulletBehavior bulletHandler = bullet.GetComponent<BulletBehavior>();
            if (bulletHandler && bulletHandler.CurrentBulletType == BulletBehavior.BulletType.enemyBullet)
            {
                bulletHandler.convertToPlayerBullet();
            } 
        }
    }
}
