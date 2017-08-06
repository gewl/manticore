using UnityEngine;

public class AggressiveBoxState : EnemyState {

    public AggressiveBoxState(EnemyStateMachine machine)
		: base(machine) { }

    int nextBulletTimer;

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
    }
}
