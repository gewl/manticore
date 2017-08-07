using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

    private float speed = 25f;
    private bool isAttached = false;

    private Rigidbody bulletRigidbody;
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;

    public Material enemyBulletSkin;
    public Material playerBulletSkin;
    public GameObject bullets;

    public enum BulletType { enemyBullet, parryingBullet, playerBullet };
    private BulletType bulletType;
    private BulletType nextType;
    public BulletType CurrentBulletType { get { return bulletType; } }

    public Vector3 lastVelocity;
    public Vector3 lockedPosition;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.forward * speed;

        bulletType = BulletType.enemyBullet;
    }

    private void Update()
    {
        Debug.Log(bulletRigidbody.velocity);
        if (bulletType == BulletType.parryingBullet)
        {
            transform.localPosition = lockedPosition;
            DrawLineToMouse();
        }
        else
        {
            lastVelocity = bulletRigidbody.velocity;
        }
    }

    #region collisionhandling
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
        if (other.gameObject.tag == "DamageZone" && bulletType == BulletType.enemyBullet)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region parrying
    public void WasParriedBy(GameObject parryObject)
    {
        bulletRigidbody.velocity = Vector3.zero;

        transform.SetParent(parryObject.transform, true);
        lockedPosition = transform.localPosition;

		if (bulletType == BulletType.playerBullet)
		{
			nextType = BulletType.enemyBullet;
		}
		else if (bulletType == BulletType.enemyBullet)
		{
			nextType = BulletType.playerBullet;
		}

		bulletType = BulletType.parryingBullet;
	}

	public void CompleteParry(float newSpeed = 1f)
	{
		bulletType = nextType;
		if (bulletType == BulletType.playerBullet)
		{
			meshRenderer.material = playerBulletSkin;
		}
		else if (bulletType == BulletType.enemyBullet)
		{
			meshRenderer.material = enemyBulletSkin;
		}
		lineRenderer.enabled = false;
        transform.SetParent(bullets.transform, true);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
			Vector3 bulletToHitpoint = (hitPoint - transform.position).normalized;

			bulletRigidbody.velocity = new Vector3(bulletToHitpoint.x, 0f, bulletToHitpoint.z) * lastVelocity.magnitude * newSpeed;
		}
	}
    #endregion

    public void DrawLineToMouse()
	{
		if (!lineRenderer.enabled)
		{
			lineRenderer.enabled = true;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;

			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, hitPoint);
		}
	}


	#region methods for explicit movement
	void Bounce(float newSpeed = 1f)
    {
        bulletRigidbody.velocity = new Vector3(bulletRigidbody.velocity.x, 0f, -bulletRigidbody.velocity.z) * newSpeed;
    }

    #endregion

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
