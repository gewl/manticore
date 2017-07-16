using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : PlayerState
{
	public MovingState(PlayerStateMachine machine)
		: base(machine) { }

	public override void Enter()
	{
		base.Enter();
	}

	public override void Update()
	{
	}

	public override void FixedUpdate()
	{
		float horizontalKeyValue = Input.GetAxis("HorizontalKey");
		float verticalKeyValue = Input.GetAxis("VerticalKey");

		Vector3 direction = new Vector3(horizontalKeyValue, 0, verticalKeyValue);
		Machine.PlayerController.ChangeVelocity(direction);

        Vector3 currentVelocity = Machine.PlayerRigidbody.velocity;

        if (Mathf.Approximately(currentVelocity.x, 0) && Mathf.Approximately(currentVelocity.z, 0)) {
            Machine.SwitchState(new StandingState(Machine));
        }
	}
}