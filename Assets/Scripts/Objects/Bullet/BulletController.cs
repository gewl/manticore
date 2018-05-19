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

    MeshRenderer _meshRenderer;
    MeshRenderer MeshRenderer
    {
        get
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }

            return _meshRenderer;
        }
    }
    TrailRenderer _trailRenderer;
    TrailRenderer TrailRenderer
    {
        get
        {
            if (_trailRenderer == null)
            {
                _trailRenderer = GetComponent<TrailRenderer>();
            }

            return _trailRenderer;
        }
    }
    Rigidbody _bulletRigidbody;
    Rigidbody BulletRigidbody
    {
        get
        {
            if (_bulletRigidbody == null)
            {
                _bulletRigidbody = GetComponent<Rigidbody>();
            }

            return _bulletRigidbody;
        }
    }

    public float speed = 25f;
    public float Strength { get; private set; }
    public void SetStrength(float newStrength)
    {
        Strength = newStrength;
        UpdateSize();
    }

    public Transform firer;
    public Transform target;
    public Vector3 targetPosition;

    bool isHoming = false;

    public void SetHoming()
    {
        isHoming = true;
        if (gameObject.CompareTag(FRIENDLY_BULLET))
        {
            GameObject targetObject = GameManager.FindNearestEnemyInFront(transform.position, transform.forward);

            if (targetObject == null)
            {
                isHoming = false;
                return;
            }
            
            target = targetObject.transform;
        }
        else
        {
            target = GameManager.GetPlayerTransform();
        }
    }

    bool isFrozen = false;

    public const string ENEMY_BULLET = "EnemyBullet";
    public const string FRIENDLY_BULLET = "FriendlyBullet";

    LayerMask _friendlyBulletLayer, _enemyBulletLayer;
    LayerMask FriendlyBulletLayer
    {
        get
        {
            if (_friendlyBulletLayer == 0)
            {
                _friendlyBulletLayer = LayerMask.NameToLayer(FRIENDLY_BULLET);
            }

            return _friendlyBulletLayer;
        }
    }
    LayerMask EnemyBulletLayer
    {
        get
        {
            if (_enemyBulletLayer == 0)
            {
                _enemyBulletLayer = LayerMask.NameToLayer(ENEMY_BULLET);
            }

            return _enemyBulletLayer;
        }
    }

    [SerializeField]
    List<LayerMask> triggerDestroyLayers;
    [SerializeField]
    GameObject enemyBulletCollisionParticles;
    [SerializeField]
    GameObject friendlyBulletCollisionParticles;
    [SerializeField]
    Transform particlesParent;

    public void InitializeValues(float _strength, Vector3 _targetPosition, Transform _firer, Transform _target, float _speed)
    {
        Strength = _strength;
        targetPosition = _targetPosition;

        if (_target != null)
        {
            target = _target;
        }
        else
        {
            target = firer;
        }

        firer = _firer;

        Physics.IgnoreCollision(GetComponent<Collider>(), firer.GetComponent<Collider>());

        speed = _speed;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 adjustedVelocity = direction * speed;
        BulletRigidbody.velocity = adjustedVelocity;
        //StartCoroutine(AccelerateBullet(direction));
        UpdateSize();
    }

    void FixedUpdate()
    {
        if (isHoming)
        {
            Vector3 normalizedVelocity = BulletRigidbody.velocity.normalized;
            Vector3 toTarget = (target.position - transform.position);
            Vector3 toTargetNormalized = toTarget.normalized;

            float angleToTarget = Vector3.Angle(normalizedVelocity, toTarget);

            Vector3 averageOfVectors = (10f * normalizedVelocity + toTargetNormalized) / 11f;

            BulletRigidbody.velocity = averageOfVectors * speed;

            if (angleToTarget >= 85f)
            {
                isHoming = false;
            }
        }

        if (!isFrozen && BulletRigidbody.velocity.sqrMagnitude < 50f)
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

        if (BulletRigidbody.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(BulletRigidbody.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 normal = collision.contacts[0].normal;
        int collisionObjectLayer = collision.gameObject.layer;

        Impact(collisionPoint, normal, collision.gameObject, collisionObjectLayer);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("collision stay");
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
            if (collisionObjectLayer != FriendlyBulletLayer && collisionObjectLayer != EnemyBulletLayer) {
                Destroy(gameObject);
            }
        }
        else
        {
            Instantiate(enemyBulletCollisionParticles, point, Quaternion.Euler(normal), particlesParent);
            Destroy(gameObject);
        }
    }

    public void Dissolve()
    {
        Vector3 currentVelocity = BulletRigidbody.velocity;
        Instantiate(enemyBulletCollisionParticles, transform.position, Quaternion.Euler(-currentVelocity), particlesParent);
        StartCoroutine(DissolveBullet());
    }

    #region Parrying
    // If no target position or strength supplied, just reverse direction.
    public void Parry(Transform newFirer)
    {
        Parry(newFirer, firer.position, Strength);
    }

    // If no target position supplied, apply new strength in reverse direction.
    public void Parry(Transform newFirer, float newStrength, float speedModifier = 2f)
    {
        Parry(newFirer, firer.position, newStrength, speedModifier);
    }

    // If no new strength supplied, maintain current strength.
    public void Parry(Transform newFirer, Vector3 targetPosition)
    {
        Parry(newFirer, targetPosition, Strength);
    }
    
    public void Parry(Transform newFirer, Vector3 targetPosition, float newStrength, float speedModifier = 2f)
    {
        GameManager.JoltScreen(BulletRigidbody.velocity);
        StopAllCoroutines();
        
        SetFriendly();

        Strength = newStrength;
        target = firer;
        firer = newFirer;
        if (targetPosition == Vector3.zero)
        {
            targetPosition = target.position; 
        }
        speed *= speedModifier;

        Vector3 direction = (targetPosition - transform.position).normalized;
        BulletRigidbody.velocity = direction * speed;

        UpdateSize();
	}
    #endregion

    public void Attach(Transform newParent, bool shouldDeactivate = true)
    {
        isFrozen = true;
        SetFriendly();
        BulletRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        transform.parent = newParent;

        if (shouldDeactivate)
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    public void Launch(Vector3 direction)
    {
        isFrozen = false;
        BulletRigidbody.constraints = RigidbodyConstraints.None;

        transform.parent = GameManager.BulletsParent.transform;

        GetComponent<Collider>().enabled = true;

        BulletRigidbody.velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(BulletRigidbody.velocity);
    }

    IEnumerator AccelerateBullet(Vector3 direction)
    {
        float timeElapsed = 0.0f;

        BulletRigidbody.velocity = direction * speed * initialSpeedModifier;
        while (timeElapsed < timeToFullSpeed)
        {
            timeElapsed += Time.deltaTime;
            float percentageCompleted = timeElapsed / timeToFullSpeed;
            float curvePoint = GameManager.BelovedSwingCurve.Evaluate(percentageCompleted);

            float fractionOfTotalSpeed = Mathf.Lerp(initialSpeedModifier, 1.0f, curvePoint);

            BulletRigidbody.velocity = direction * speed * fractionOfTotalSpeed;
            yield return null;
        }
        BulletRigidbody.velocity = direction * speed;
    }

    public void SetFriendly()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), firer.GetComponent<Collider>(), false);

        gameObject.layer = FriendlyBulletLayer;
        gameObject.tag = FRIENDLY_BULLET;

        MeshRenderer.material = friendlyBulletMaterial;
		TrailRenderer.material = friendlyBulletMaterial;
    }

    void UpdateSize()
    {
        if (Strength == 0f)
        {
            return;
        }
        else
        {
            float proportionToDefault = Strength / defaultStrength;
            float strengthScale = Mathf.Atan(proportionToDefault * Mathf.PI / 2);
            strengthScale = Mathf.Pow(strengthScale, 2f);
            transform.localScale *= strengthScale;
        }
    }

    IEnumerator DissolveBullet()
    {
        isFrozen = true;
        Color materialColor = MeshRenderer.material.color;

        BulletRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = false;

        MeshRenderer.material = new Material(dissolveBulletMaterial)
        {
            color = materialColor
        };

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToDestroyDissolvingBullet)
        {
            timeElapsed += Time.deltaTime;
            MeshRenderer.material.SetFloat("_TimeElapsed", timeElapsed);
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }

}