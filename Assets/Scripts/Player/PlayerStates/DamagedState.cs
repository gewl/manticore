using System.Collections.Generic;
using UnityEngine;

public class DamagedState : PlayerState
{
    Vector3 bulletVelocity;

	public DamagedState(PlayerStateMachine machine, Vector3 colliderVelocity)
		: base(machine)
    {
        bulletVelocity = colliderVelocity;
    }

    private int damagedTimer = 31;

    private GameObject body;

	public override void Enter()
	{
		base.Enter();
        body = Machine.Player.transform.GetChild(0).gameObject;
        Machine.PlayerController.ChangeVelocity(bulletVelocity, 0.5f);
	}

	public override void Update()
	{
        if (damagedTimer % 20 == 0) 
        {
            body.SetActive(true);
        } 
        else if (damagedTimer % 10 == 0)
        {
            body.SetActive(false);     
        }
		if (damagedTimer <= 0)
		{
            Machine.PlayerController.Stop();
            Machine.SwitchState(new StandardState(Machine));
		}
        damagedTimer--;
	}
}