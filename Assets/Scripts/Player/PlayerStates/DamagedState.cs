using System.Collections.Generic;
using UnityEngine;

public class DamagedState : PlayerState
{
	public DamagedState(PlayerStateMachine machine)
		: base(machine) { }

    private int damagedTimer = 30;

    private GameObject body;

	public override void Enter()
	{
		base.Enter();
        body = Machine.Player.transform.GetChild(0).gameObject;
	}

	public override void Update()
	{
        Debug.Log(damagedTimer);
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
            Machine.SwitchState(new StandingState(Machine));
		}
        damagedTimer--;
	}
}