using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    private float speed = 25f;
    private Rigidbody bulletRigidbody;
    private MeshRenderer meshRenderer;

    public Material enemyBulletSkin;
    public Material playerBulletSkin;

    public enum BulletType { enemyBullet, playerBullet };
    private BulletType bulletType;
    public BulletType CurrentBulletType { get { return bulletType; } }

    public Vector3 lastVelocity;

	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.forward * speed;

        bulletType = BulletType.enemyBullet;
	}

    private void Update()
    {
        lastVelocity = bulletRigidbody.velocity;
    }

    void Bounce(float speed = 1f) {
        bulletRigidbody.velocity = new Vector3(bulletRigidbody.velocity.x, 0f, -bulletRigidbody.velocity.z) * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (bulletType == BulletType.enemyBullet)
            {
                Destroy(gameObject); 
            }
            else if (bulletType == BulletType.playerBullet)
            {
                bulletRigidbody.velocity = lastVelocity * 1.3f;
            }
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DamageZone")
        {
            Destroy(gameObject);
        }
    }

    public void convertToPlayerBullet()
    {
        if (bulletType == BulletType.enemyBullet)
        {
            Bounce(1.4f);
            bulletType = BulletType.playerBullet;
            meshRenderer.material = playerBulletSkin;
        }
    }

    public bool IsFriendly(GameObject go)
    {
        if (go.tag == "Player")
        {
            if (bulletType == BulletType.playerBullet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (bulletType == BulletType.enemyBullet)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
