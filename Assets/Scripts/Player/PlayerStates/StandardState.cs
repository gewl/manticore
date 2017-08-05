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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Machine.PlayerController.Parry();
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
		if (colliderGo.tag == "Bullet" && !colliderGo.GetComponent<BulletBehavior>().IsFriendly(Machine.Player))
		{
			Machine.PlayerController.ChangeVelocity(co.attachedRigidbody.velocity, false);
            Debug.Log("Ouch!");
			Machine.SwitchState(new DamagedState(Machine));
		}
	}
}