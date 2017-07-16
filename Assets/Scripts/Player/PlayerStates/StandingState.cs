using UnityEngine;

public class StandingState : PlayerState
{
	public StandingState(PlayerStateMachine machine)
		: base(machine) { }

	public override void Enter()
	{
		base.Enter();

        Machine.PlayerController.Stop();
	}

	public override void Update()
	{
		float horizontalKeyValue = Input.GetAxis("HorizontalKey");
		float verticalKeyValue = Input.GetAxis("VerticalKey");

        if (System.Math.Abs(verticalKeyValue) > 0f || System.Math.Abs(horizontalKeyValue) > 0f)
		{
            Machine.SwitchState(new MovingState(Machine));
		}
	}

}