using UnityEngine;

public class DeadBoxState : EnemyState
{

    Vector3 bulletVelocity;

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
	}

	public override void Update()
	{
        
	}
}
