using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {

    [SerializeField]
    Material enemyBulletMaterial;
    [SerializeField]
    Material friendlyBulletMaterial;

    MeshRenderer meshRenderer;
    TrailRenderer trailRenderer;
    Rigidbody bulletRigidbody;

    [SerializeField]
    float speed = 25f;
    int strength = 1;

    public Transform firer;
    public Transform target;

    enum BulletType {
        FriendlyBullet,
        EnemyBullet
    }

    BulletType currentBulletType = BulletType.EnemyBullet;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        bulletRigidbody.velocity = transform.forward * speed;
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    public void Parry(Transform newFirer)
    {
        gameObject.layer = 12;
        gameObject.tag = "FriendlyBullet";
        bulletRigidbody.velocity = -bulletRigidbody.velocity;

        meshRenderer.material = friendlyBulletMaterial;
		trailRenderer.material = friendlyBulletMaterial;

        firer = newFirer;
        currentBulletType = BulletType.FriendlyBullet;
	}

}
