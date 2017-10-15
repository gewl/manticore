using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{

    private float speed = 25f;
    private int damageValue = 1;
    private Collider lastWallHit;
    private Material nextSkin;
    private Material currentSkin;

    private Rigidbody bulletRigidbody;
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;

    public Material enemyBulletSkin;
    public Material playerBulletSkin;
    private GameObject bullets;

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
        bullets = transform.parent.gameObject;
    }

    private void Update()
    {
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "BouncyWall")
        {
            if (lastWallHit != collision.collider)
            {
                IncreaseDamageValue();
                lastWallHit = collision.collider;
            }
        }
    }

    private void IncreaseDamageValue()
    {
        if (damageValue >= 5) 
        {
            return;
        }

        damageValue++;
        transform.localScale *= 1.2f;
        bulletRigidbody.velocity *= 1.2f;
    }

    private void ResetDamageValue()
    {
        damageValue = 1;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    #region parrying
    public void WasParriedBy(GameObject parryObject)
    {
        bulletRigidbody.velocity = Vector3.zero;

        transform.SetParent(parryObject.transform, true);
        lockedPosition = transform.localPosition;
        currentSkin = meshRenderer.material;

		if (bulletType == BulletType.playerBullet)
		{
			nextType = BulletType.enemyBullet;
            nextSkin = enemyBulletSkin;
		}
		else if (bulletType == BulletType.enemyBullet)
		{
			nextType = BulletType.playerBullet;
            nextSkin = playerBulletSkin;
		}

        ResetDamageValue();
		bulletType = BulletType.parryingBullet;
	}

	public void CompleteParry(float newSpeed = 1f)
	{
		bulletType = nextType;
        meshRenderer.material = nextSkin;
		lineRenderer.enabled = false;
        transform.SetParent(bullets.transform, true);

        Vector3 mousePosition = GameManager.GetMousePositionInWorldSpace();
        Vector3 bulletToHitpoint = (mousePosition - transform.position).normalized;

        bulletRigidbody.velocity = new Vector3(bulletToHitpoint.x, 0f, bulletToHitpoint.z) * lastVelocity.magnitude * newSpeed;
	}

    public void UpdateMaterial(float percentageDone) 
    {
        meshRenderer.material.Lerp(currentSkin, nextSkin, percentageDone);     
    }

    #endregion

    public void DrawLineToMouse()
	{
		if (!lineRenderer.enabled)
		{
			lineRenderer.enabled = true;
		}
        Vector3 mousePosition = GameManager.GetMousePositionInWorldSpace();
		if (mousePosition != Vector3.zero)
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, mousePosition);
		}
	}


	#region methods for explicit movement
	void Bounce(float newSpeed = 1f)
    {
        bulletRigidbody.velocity = new Vector3(bulletRigidbody.velocity.x, 0f, -bulletRigidbody.velocity.z) * newSpeed;
    }

    #endregion

    public bool IsUnfriendly(Transform entity)
    {
        if (entity.tag == "Player")
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
        else
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
    }

    public bool IsUnfriendly(GameObject go)
    {
        if (go.tag == "Player")
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
        else
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
    }
}
