using UnityEngine;

public class SuccessfulParryState : PlayerState
{
    GameObject parriedBullet;
    BulletBehavior bulletHandler;
	private int timer;

	public SuccessfulParryState(PlayerStateMachine machine, GameObject bullet)
		: base(machine) 
    {
        parriedBullet = bullet;
    }

	public override void Enter()
	{
		base.Enter();
        if (parriedBullet != null)
        {
			bulletHandler = parriedBullet.GetComponent<BulletBehavior>();
		}

		timer = 20;
		Machine.PlayerController.parryBox.SetActive(true);
	}

	public override void Update()
	{
        if (bulletHandler == null)
        {
            Machine.SwitchState(new StandardState(Machine));
        }
        if (timer > 0)
		{
			timer--;
		}
		else
		{
            bulletHandler.CompleteParry(1.4f);
			Machine.SwitchState(new StandardState(Machine));
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
			Machine.PlayerController.ChangeVelocity(direction, 0.5f);
		}
	}

	public override void Exit()
	{
		base.Exit();
		Machine.PlayerController.parryBox.SetActive(false);
	}

	public override void HandleTriggerEnter(Collider co)
	{
		GameObject colliderGo = co.gameObject;
		if (colliderGo.tag == "Bullet" && colliderGo.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Player))
		{
			Machine.PlayerController.ChangeVelocity(co.attachedRigidbody.velocity, 0.7f);
			Debug.Log("Ouch!");
			Machine.SwitchState(new DamagedState(Machine));
		}
	}
}