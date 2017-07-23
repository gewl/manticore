using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoxController : MonoBehaviour {

    int nextBulletTimer = 100;
    public Transform bullet;

    private Transform bulletSpawner;

    private Collider boxCollider;

	void Start () 
    {
        bulletSpawner = transform.Find("BulletSpawner");

        boxCollider = transform.Find("Body").GetComponent<Collider>();
	}
	
	void Update () 
    {
        nextBulletTimer--;

        if (nextBulletTimer == 0)
        {
            nextBulletTimer = 100;

            Instantiate(bullet, bulletSpawner.position, bulletSpawner.rotation);
            //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), boxCollider);
        }
    }
}
