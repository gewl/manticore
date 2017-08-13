using UnityEngine;

public class AggressiveBoxState : EnemyState {

    public AggressiveBoxState(EnemyStateMachine machine)
		: base(machine) { }

    int nextBulletTimer;
    float rotateStrength = 3f;

    public override void Enter () {
        nextBulletTimer = 100;
	}
	
	public override void Update () {
        nextBulletTimer--;

        if (nextBulletTimer == 0)
        {
            Machine.EnemyController.Attack();
            nextBulletTimer = 100;
        }

        Machine.EnemyController.RotateToFace(Machine.Player);
    }

    public override void HandleTriggerEnter(Collider co)
    {
        base.HandleTriggerEnter(co);
        if (co.gameObject.tag == "Bullet" && co.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Enemy)) {
			Object.Destroy(co.gameObject);
            Machine.SwitchState(new DeadBoxState(Machine, co.attachedRigidbody.velocity));
		}
    }
}
