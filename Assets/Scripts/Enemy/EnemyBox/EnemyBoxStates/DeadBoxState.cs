﻿using UnityEngine;

public class DeadBoxState : EnemyState
{

    Vector3 bulletVelocity;
    float damagedTimer = 55f;
    float originalTimer;
    GameObject body;
    Material originalSkin;

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

        originalSkin = Machine.EnemyController.MeshRenderer.material;
        originalTimer = damagedTimer;
	}

    public override void Update()
    {
        if (damagedTimer >= 0)
        {
            //if (System.Math.Abs(damagedTimer % 20) < 0.1f)
            //{
            //    body.SetActive(true);
            //}
            //else if (System.Math.Abs(damagedTimer % 10) < 0.1f)
            //{
            //    body.SetActive(false);
            //}

            float percentageDone = (originalTimer - damagedTimer) / originalTimer;
            percentageDone = Mathf.Pow(percentageDone, 2f);
            Machine.EnemyController.MeshRenderer.material.Lerp(originalSkin, Machine.EnemyController.deathSkin, percentageDone);

			damagedTimer--;
		}
        else
        {
            Machine.EnemyRigidbody.detectCollisions = false;
            Machine.EnemyRigidbody.drag = 10f;
            Machine.EnemyRigidbody.freezeRotation = true;
            Machine.EnemyRigidbody.AddForce(new Vector3(0f, -100f, 0f));
            if (Machine.Enemy.transform.position.y <= -2f)
            {
                Object.Destroy(Machine.Enemy);
            }
        }
	}
}
