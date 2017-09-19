using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoxController : EnemyController
{
    // assigned in editor
    public Transform bullet;

    // assigned in Start function
    private Transform bulletSpawner;
    private GameObject bullets;

    [SerializeField]
    int startingHealth;
    public override int StartingHealth { get { return startingHealth; } }

    private MeshRenderer meshRenderer;
    public override MeshRenderer MeshRenderer { get { return meshRenderer; }}

    private EnemyStateMachine enemyMachine;
    public override EnemyStateMachine EnemyMachine { get { return enemyMachine; } }

    private void Awake()
    {
        enemyMachine = ScriptableObject.CreateInstance<EnemyStateMachine>();
        enemyMachine.Init(gameObject, this, GetComponent<Rigidbody>());
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    void Start()
    {
        bulletSpawner = transform.Find("BulletSpawner");
        bullets = GameObject.Find("Bullets");
    }

    void Update()
    {
        enemyMachine.Update();
    }

    public override void Attack()
    {
        Transform createdBullet = Object.Instantiate(bullet, bulletSpawner.position, bulletSpawner.rotation);
        createdBullet.transform.parent = bullets.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyMachine.HandleTriggerEnter(other);
    }
}
