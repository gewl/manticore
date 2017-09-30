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
    public float strength = 1;

    public Transform firer;
    public Transform target;
    public Vector3 targetPosition;

    enum BulletType {
        FriendlyBullet,
        EnemyBullet
    }

    BulletType currentBulletType = BulletType.EnemyBullet;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        bulletRigidbody.velocity = targetPosition.normalized * speed;
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

    public void Parry(Transform newFirer, Vector3 targetPosition)
    {
        target = firer;
        firer = newFirer;
        if (targetPosition == Vector3.zero)
        {
            targetPosition = target.position; 
        }
        gameObject.layer = 12;
        gameObject.tag = "FriendlyBullet";
        bulletRigidbody.velocity = (targetPosition - transform.position).normalized * speed * 2f;

        meshRenderer.material = friendlyBulletMaterial;
		trailRenderer.material = friendlyBulletMaterial;

        currentBulletType = BulletType.FriendlyBullet;
	}

}
