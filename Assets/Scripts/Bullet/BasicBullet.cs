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

    public float speed = 5f;
    public int strength = 1;

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

        bulletRigidbody.velocity = (target.transform.position - firer.position).normalized * speed;
	}

    private void OnCollisionEnter(Collision collision)
    {
		Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
			Destroy(gameObject);
		}
	}

    public void Parry(Transform newFirer)
    {
        gameObject.layer = 12;
        gameObject.tag = "FriendlyBullet";
        target = firer;
        bulletRigidbody.velocity = (target.transform.position - newFirer.position).normalized * speed * 2f;

        meshRenderer.material = friendlyBulletMaterial;
		trailRenderer.material = friendlyBulletMaterial;

        firer = newFirer;
        currentBulletType = BulletType.FriendlyBullet;
	}

}
