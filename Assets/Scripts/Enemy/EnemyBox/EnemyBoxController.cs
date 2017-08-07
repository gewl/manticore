using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoxController : EnemyController {

    // assigned in editor
    public Transform bullet;

    // assigned in Start function
    private Transform bulletSpawner;
    private Collider boxCollider;
    private GameObject bullets;

    private EnemyStateMachine enemyMachine;

    private void Awake()
    {
        enemyMachine = ScriptableObject.CreateInstance<EnemyStateMachine>();
        enemyMachine.init(gameObject, this);
    }

	void Start () 
    {
        bulletSpawner = transform.Find("BulletSpawner");
        boxCollider = transform.Find("Body").GetComponent<Collider>();
        bullets = GameObject.Find("Bullets");
	}
	
	void Update () 
    {
        enemyMachine.Update();
    }

    public override void Attack()
    {
        Transform createdBullet = GameObject.Instantiate(bullet, bulletSpawner.position, bulletSpawner.rotation);
        createdBullet.transform.parent = bullets.transform;
    }
}
