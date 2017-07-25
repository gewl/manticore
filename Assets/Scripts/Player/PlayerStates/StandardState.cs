using UnityEngine;

public class StandardState : PlayerState
{
	public StandardState(PlayerStateMachine machine)
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
		if (co.gameObject.tag == "Bullet")
		{
			Machine.PlayerController.ChangeVelocity(co.attachedRigidbody.velocity, false);
			Machine.SwitchState(new DamagedState(Machine));
		}
	}
}