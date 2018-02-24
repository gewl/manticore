using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [SerializeField]
    Material friendlyBulletMaterial;
    [SerializeField]
    Material dissolveBulletMaterial;
    [SerializeField]
    float defaultStrength = 50f;
    [SerializeField]
    float timeToDestroyDissolvingBullet = 2.0f;

    [SerializeField]
    float timeToFullSpeed = 0.3f;
    [SerializeField]
    float initialSpeedModifier = 0.5f;

    MeshRenderer meshRenderer;
    TrailRenderer trailRenderer;
    Rigidbody bulletRigidbody;

    public float speed = 5f;
    public float strength;

    public Transform firer;
    public Transform target;
    public Vector3 targetPosition;

    public bool IsHoming = false;
    bool isFrozen = false;

    public const string ENEMY_BULLET = "EnemyBullet";
    public const string FRIENDLY_BULLET = "FriendlyBullet";
    public const string NULLIFIER_LAYER = "Nullifier";

    [SerializeField]
    List<LayerMask> triggerDestroyLayers;
    [SerializeField]
    GameObject enemyBulletCollisionParticles;
    [SerializeField]
    GameObject friendlyBulletCollisionParticles;
    [SerializeField]
    Transform particlesParent;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        Vector3 direction = (targetPosition - transform.position).normalized;
        StartCoroutine(AccelerateBullet(direction));
        UpdateSize();
	}

    void FixedUpdate()
    {
        if (IsHoming)
        {
            Vector3 normalizedVelocity = bulletRigidbody.velocity.normalized;
            Vector3 toTarget = (target.position - transform.position);
            Vector3 toTargetNormalized = toTarget.normalized;

            float angleToTarget = Vector3.Angle(normalizedVelocity, toTarget);

            Vector3 averageOfVectors = (10f * normalizedVelocity + toTargetNormalized) / 11f;

            bulletRigidbody.velocity = averageOfVectors * speed;

            if (angleToTarget >= 85f)
            {
                IsHoming = false;
            }
        }

        if (!isFrozen && bulletRigidbody.velocity.sqrMagnitude < 150f)
        {
            if (gameObject.CompareTag(FRIENDLY_BULLET))
            {
                Instantiate(friendlyBulletCollisionParticles, transform.position, Quaternion.identity, particlesParent);
            }
            else
            {
                Instantiate(enemyBulletCollisionParticles, transform.position, Quaternion.identity, particlesParent);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 normal = collision.contacts[0].normal;
        int collisionObjectLayer = collision.gameObject.layer;

        Impact(collisionPoint, normal, collision.gameObject, collisionObjectLayer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerDestroyLayers.Contains(other.gameObject.layer))
        {
			Destroy(gameObject);
		}
	}

    protected virtual void Impact(Vector3 point, Vector3 normal, GameObject collisionObject, int collisionObjectLayer)
    {
        if (gameObject.CompareTag(FRIENDLY_BULLET))
        {
            Instantiate(friendlyBulletCollisionParticles, point, Quaternion.Euler(normal), particlesParent);
            if (collisionObjectLayer != 9) {
                Destroy(gameObject);
            }
        }
        else if (collisionObjectLayer == LayerMask.NameToLayer(NULLIFIER_LAYER))
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), particlesParent);
            StartCoroutine(DissolveBullet());
        }
        else
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), particlesParent);
            Destroy(gameObject);
        }
    }

    // If no target position or strength supplied, just reverse direction.
    public void Parry(Transform newFirer)
    {
        Parry(newFirer, firer.position, strength);
    }

    // If no target position supplied, apply new strength in reverse direction.
    public void Parry(Transform newFirer, float newStrength, float speedModifier = 2f)
    {
        Parry(newFirer, firer.position, newStrength, speedModifier);
    }

    // If no new strength supplied, maintain current strength.
    public void Parry(Transform newFirer, Vector3 targetPosition)
    {
        Parry(newFirer, targetPosition, strength);
    }
    
    public void Parry(Transform newFirer, Vector3 targetPosition, float newStrength, float speedModifier = 2f)
    {
        GameManager.JoltScreen(bulletRigidbody.velocity);
        
        strength = newStrength;
        target = firer;
        firer = newFirer;
        if (targetPosition == Vector3.zero)
        {
            targetPosition = target.position; 
        }
        gameObject.layer = 12;
        gameObject.tag = FRIENDLY_BULLET;
        speed *= speedModifier;

        Vector3 direction = (targetPosition - transform.position).normalized;
        bulletRigidbody.velocity = direction * speed;

        meshRenderer.material = friendlyBulletMaterial;
		trailRenderer.material = friendlyBulletMaterial;
        UpdateSize();
	}

    IEnumerator AccelerateBullet(Vector3 direction)
    {
        float timeElapsed = 0.0f;

        bulletRigidbody.velocity = direction * speed * initialSpeedModifier;
        while (timeElapsed < timeToFullSpeed)
        {
            timeElapsed += Time.deltaTime;
            float percentageCompleted = timeElapsed / timeToFullSpeed;
            float curvePoint = GameManager.BelovedSwingCurve.Evaluate(percentageCompleted);

            float fractionOfTotalSpeed = Mathf.Lerp(initialSpeedModifier, 1.0f, curvePoint);

            bulletRigidbody.velocity = direction * speed * fractionOfTotalSpeed;
            yield return null;
        }
        bulletRigidbody.velocity = direction * speed;
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

    IEnumerator DissolveBullet()
    {
        isFrozen = true;
        Color materialColor = meshRenderer.material.color;

        bulletRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = false;

        meshRenderer.material = new Material(dissolveBulletMaterial)
        {
            color = materialColor
        };

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToDestroyDissolvingBullet)
        {
            timeElapsed += Time.deltaTime;
            meshRenderer.material.SetFloat("_TimeElapsed", timeElapsed);
            yield return null;
        }
        Debug.Log("destroying");

        Destroy(gameObject);
        yield break;
    }

}