using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHandler : MonoBehaviour {

    public GameObject manticore;

    public delegate void ParryEventHandler(GameObject bullet);
    public event ParryEventHandler ParriedBullet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            GameObject bullet = other.gameObject;
            BulletBehavior bulletHandler = bullet.GetComponent<BulletBehavior>();
            if (bulletHandler && bulletHandler.CurrentBulletType == BulletBehavior.BulletType.enemyBullet)
            {
                //bulletHandler.WasParriedBy(manticore);
                ParriedBullet(bullet);
            } 
        }
    }
}
