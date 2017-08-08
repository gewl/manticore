using UnityEngine;

public class DeadBoxState : EnemyState
{

    Vector3 bulletVelocity;
	int damagedTimer = 55;
    GameObject body;

	public DeadBoxState(EnemyStateMachine machine, Vector3 colliderVelocity)
		: base(machine) 
    {
        bulletVelocity = colliderVelocity;
    }

	public override void Enter()
	{
        Debug.Log(bulletVelocity);
        Machine.Enemy.transform.position = new Vector3(Machine.Enemy.transform.position.x, Machine.Enemy.transform.position.y + 1f, Machine.Enemy.transform.position.z);
        Machine.EnemyRigidbody.constraints = RigidbodyConstraints.None;
        Machine.EnemyRigidbody.useGravity = true;
        Machine.EnemyRigidbody.isKinematic = false;
        Machine.EnemyCollider.isTrigger = false;

        Machine.EnemyRigidbody.AddForce(bulletVelocity, ForceMode.Impulse);
        Machine.EnemyRigidbody.AddTorque(bulletVelocity.z, 0f, -bulletVelocity.x, ForceMode.Impulse);

        body = Machine.Enemy.transform.GetChild(0).gameObject;

        Machine.EnemyController.MeshRenderer.material = Machine.EnemyController.deathSkin;
	}

    public override void Update()
    {
        if (damagedTimer >= 0)
        {
            if (damagedTimer % 20 == 0)
            {
                body.SetActive(true);
            }
            else if (damagedTimer % 10 == 0)
            {
                body.SetActive(false);
            }

			damagedTimer--;
		}
        else
        {
            Machine.EnemyRigidbody.detectCollisions = false;
            Machine.EnemyRigidbody.drag = 10f;
            Machine.EnemyRigidbody.freezeRotation = true;
            Machine.EnemyRigidbody.AddForce(new Vector3(0f, -100f, 0f));
            Debug.Log(Machine.Enemy.transform.position.y);
            if (Machine.Enemy.transform.position.y <= -2f)
            {
                Object.Destroy(Machine.Enemy);
            }
        }
	}
}
