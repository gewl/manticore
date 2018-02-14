using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour {

    [SerializeField]
    Material enemyBulletMaterial;
    [SerializeField]
    Material friendlyBulletMaterial;
    [SerializeField]
    Material dissolveBulletMaterial;
    [SerializeField]
    float defaultStrength = 50f;
    [SerializeField]
    float timeToDestroyDissolvingBullet = 2.0f;

    [SerializeField]
    float timeToFullSpeedOnParry = 0.3f;
    [SerializeField]
    float initialSpeedModifierOnParry = 0.5f;

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
    Transform effectParent;

	void Start () {
        bulletRigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        //bulletRigidbody.velocity = targetPosition.normalized * speed;
        StartCoroutine(AccelerateBullet(targetPosition));
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
        if (triggerDestroyLayers.Contains(other.gameObject.layer))
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
        else if (collisionObjectLayer == LayerMask.NameToLayer(NULLIFIER_LAYER))
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), effectParent);
            StartCoroutine(DissolveBullet());
        }
        else
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), effectParent);
            Destroy(gameObject);
        }
    }

    // If no target position or strength supplied, just reverse direction.
    public void Parry(Transform newFirer)
    {
        Parry(newFirer, firer.position, strength);
    }

    public void Parry(Transform newFirer, float newStrength, float speedModifier = 2f)
    {
        Parry(newFirer, firer.position, newStrength, speedModifier);
    }

    // If no new strength supplied, executes parry maintaining current strength.
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
        StartCoroutine(AccelerateBullet(targetPosition));

        meshRenderer.material = friendlyBulletMaterial;
		trailRenderer.material = friendlyBulletMaterial;
        UpdateSize();
	}

    IEnumerator AccelerateBullet(Vector3 targetPosition)
    {
        float timeElapsed = 0.0f;
        Vector3 velocityDirection = (targetPosition - transform.position).normalized;

        while (timeElapsed < timeToFullSpeedOnParry)
        {
            timeElapsed += Time.deltaTime;
            float percentageCompleted = timeElapsed / timeToFullSpeedOnParry;
            float curvePoint = GameManager.BelovedSwingCurve.Evaluate(percentageCompleted);

            float fractionOfTotalSpeed = Mathf.Lerp(initialSpeedModifierOnParry, 1.0f, curvePoint);

            bulletRigidbody.velocity = velocityDirection * speed * fractionOfTotalSpeed;
            yield return null;
        }
        bulletRigidbody.velocity = velocityDirection * speed;
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