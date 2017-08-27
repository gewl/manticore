﻿using UnityEngine;

public class StandardState : PlayerState
{
	public StandardState(PlayerStateMachine machine)
		: base(machine) { }

    int actionCooldown;

	public override void Enter()
	{
		base.Enter();
        actionCooldown = 20;
	}

	public override void Update()
	{
        if (actionCooldown > 0)
        {
            actionCooldown--;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Machine.SwitchState(new AttemptingParryState(Machine));
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Machine.SwitchState(new BlinkingState(Machine));
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            Vector3 characterToHitpoint = (hitPoint - Machine.Player.transform.position).normalized;

            Machine.PlayerController.UpdateBodyRotation(characterToHitpoint);
		}
	}

	public override void FixedUpdate()
	{
		float horizontalKeyValue = Input.GetAxis("HorizontalKey");
		float verticalKeyValue = Input.GetAxis("VerticalKey");

        Vector3 currentVelocity = Machine.PlayerRigidbody.velocity;

        if (Mathf.Abs(horizontalKeyValue) < 0.1f && Mathf.Abs(verticalKeyValue) < 0.1f)
		{
            Machine.PlayerController.Stop();
		}
        else
        {
			Vector3 direction = new Vector3(horizontalKeyValue, 0, verticalKeyValue);
			Machine.PlayerController.ChangeVelocity(direction);
		}
    }

	public override void HandleTriggerEnter(Collider co)
	{
        GameObject colliderGo = co.gameObject;
		if (colliderGo.tag == "Bullet" && colliderGo.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Player))
		{
            Debug.Log("Ouch!");
			Object.Destroy(co.gameObject);
			Machine.SwitchState(new DamagedState(Machine, co.attachedRigidbody.velocity));
		}
	}
}