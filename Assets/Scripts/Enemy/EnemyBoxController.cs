using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoxController : MonoBehaviour {

    // initialized on instantiation
    int nextBulletTimer = 100;

    // assigned in editor
    public Transform bullet;

    // assigned in Start function
    private Transform bulletSpawner;
    private Collider boxCollider;
    private GameObject bullets;

	void Start () 
    {
        bulletSpawner = transform.Find("BulletSpawner");
        boxCollider = transform.Find("Body").GetComponent<Collider>();
        bullets = GameObject.Find("Bullets");                                                        
	}
	
	void Update () 
    {
        nextBulletTimer--;

        if (nextBulletTimer == 0)
        {
            nextBulletTimer = 100;

            Transform createdBullet = Instantiate(bullet, bulletSpawner.position, bulletSpawner.rotation);
            createdBullet.transform.parent = bullets.transform;
        }
    }
}
