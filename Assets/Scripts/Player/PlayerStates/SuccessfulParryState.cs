using UnityEngine;

public class SuccessfulParryState : PlayerState
{
    GameObject parriedBullet;
    BulletBehavior bulletHandler;
	private float timer;
    private float originalTimer;

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

        timer = 20f;
        originalTimer = timer;
		Machine.PlayerController.parryBox.SetActive(true);
        bulletHandler.WasParriedBy(Machine.Player);
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
            float percentageDone = (originalTimer - timer) / originalTimer;
			percentageDone = Mathf.Pow(percentageDone, 2f);

            bulletHandler.UpdateMaterial(percentageDone);
		}
		else
		{
            bulletHandler.CompleteParry(1.4f);
			Machine.SwitchState(new StandardState(Machine));
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
            Debug.Log("Ouch!");
			Object.Destroy(co.gameObject);
			Machine.SwitchState(new DamagedState(Machine, co.attachedRigidbody.velocity));
		}
	}
}