﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {

    [SerializeField]
    Material enemyBulletMaterial;
    [SerializeField]
    Material friendlyBulletMaterial;
    [SerializeField]
    float defaultStrength = 50f;

    MeshRenderer meshRenderer;
    TrailRenderer trailRenderer;
    Rigidbody bulletRigidbody;

    public float speed = 5f;
    public float strength;

    public Transform firer;
    public Transform target;
    public Vector3 targetPosition;

    const string ENEMY_BULLET = "EnemyBullet";
    const string FRIENDLY_BULLET = "FriendlyBullet";

    [SerializeField]
    GameObject enemyBulletCollisionParticles;
    [SerializeField]
    GameObject friendlyBulletCollisionParticles;
    [SerializeField]
    Transform effectParent;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        bulletRigidbody.velocity = targetPosition.normalized * speed;
        UpdateSize();
	}

    void FixedUpdate()
    {
        if (bulletRigidbody.velocity.sqrMagnitude < 150f)
        {
            if (gameObject.CompareTag(FRIENDLY_BULLET))
            {
                Instantiate(friendlyBulletCollisionParticles, transform.position, Quaternion.identity, effectParent);
            }
            else
            {
                Instantiate(enemyBulletCollisionParticles, transform.position, Quaternion.identity, effectParent);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 normal = collision.contacts[0].normal;
        Impact(collisionPoint, normal, collision.gameObject.layer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
			Destroy(gameObject);
		}
	}

    void Impact(Vector3 point, Vector3 normal, int collisionObjectLayer)
    {
        if (gameObject.CompareTag(FRIENDLY_BULLET))
        {
            Instantiate(friendlyBulletCollisionParticles, point, Quaternion.Euler(normal), effectParent);
            if (collisionObjectLayer != 9) {
                Destroy(gameObject);
            }
        }
        else
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), effectParent);
            Destroy(gameObject);
        }
    }

    // If no new strength supplied, executes parry maintaining current strength.
    public void Parry(Transform newFirer, Vector3 targetPosition)
    {
        Parry(newFirer, targetPosition, strength);
    }
    
    public void Parry(Transform newFirer, Vector3 targetPosition, float newStrength)
    {
        strength = newStrength;
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
        UpdateSize();
	}

    void UpdateSize()
    {
        if (strength == 0f)
        {
            strength = defaultStrength;
        }
        else
        {
            float proportionToDefault = strength / defaultStrength;
            float strengthScale = Mathf.Atan(proportionToDefault * Mathf.PI / 2);
            strengthScale = Mathf.Pow(strengthScale, 2f);
            transform.localScale *= strengthScale;
        }
    }

}
