// This was originally the base state, but I was starting to put redundant code in in between this and MovingState.
// Even though I think that it will be important to differentiate between moving/standing still for attack animations/
// other combat behaviors later on, going to try collapsing MovingState into a single business-as-usual state to cut 
// down on redundant code--keeping this around for a commit or two in case I change my mind!

//using UnityEngine;

//public class StandingState : PlayerState
//{
//	public StandingState(PlayerStateMachine machine)
//		: base(machine) { }

//	public override void Enter()
//	{
//		base.Enter();

//        Machine.PlayerController.Stop();
//	}

//	public override void Update()
//	{
//		float horizontalKeyValue = Input.GetAxis("HorizontalKey");
//		float verticalKeyValue = Input.GetAxis("VerticalKey");

//        if (System.Math.Abs(verticalKeyValue) > 0f || System.Math.Abs(horizontalKeyValue) > 0f)
//		{
//            Machine.SwitchState(new MovingState(Machine));
//		}
//	}

//    public override void HandleTriggerEnter(Collider co) 
//    {
//        if (co.gameObject.tag == "Bullet")
//        {
//            Machine.PlayerController.ChangeVelocity(co.attachedRigidbody.velocity, false);
//            Machine.SwitchState(new DamagedState(Machine));
//        }
//    }
//}