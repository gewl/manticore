using UnityEngine;

public class BoxMiddleState : EnemyState {

    public BoxMiddleState(EnemyStateMachine machine, int nextBulletTimer = 20)
		: base(machine)
    {
        _nextBulletTimer = nextBulletTimer;
    }

    int _nextBulletTimer;
    int bulletSpeed = 60;
    float rotateStrength = 3f;
    Vector3 nextCheckpoint;

    public override void Enter () {
        Vector3 positionDifference = Machine.Enemy.transform.position - Machine.Player.transform.position;
        nextCheckpoint = Machine.EnemyController.GenerateCombatMovementPosition(Machine.Player.transform.position, positionDifference);
        Machine.EnemyController.ChangeVelocity(nextCheckpoint - Machine.Enemy.transform.position);
	}
	
	public override void Update () {
        _nextBulletTimer--;

        if (_nextBulletTimer == 0)
        {
            Machine.EnemyController.Attack();
            _nextBulletTimer = bulletSpeed;
        }

        Vector3 positionDifference = Machine.Enemy.transform.position - Machine.Player.transform.position;
        float positionDifferenceMagnitude = positionDifference.magnitude;

        if (positionDifferenceMagnitude < 15f)
        {
            Machine.SwitchState(new BoxCloseState(Machine));
        }
        else 
        {
            Machine.EnemyController.RotateToFace(Machine.Player, rotateStrength);

            if (Mathf.Abs(Machine.Enemy.transform.position.x - nextCheckpoint.x) <= 0.3f && Mathf.Abs(Machine.Enemy.transform.position.z - nextCheckpoint.z) <= 0.3f)
            {
                Machine.SwitchState(new BoxWaitingState(Machine, 20, _nextBulletTimer, bulletSpeed));
            }
        }
    }

    public override void HandleTriggerEnter(Collider co)
    {
        base.HandleTriggerEnter(co);
        if (co.gameObject.tag == "Bullet" && co.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Enemy)) {
			Object.Destroy(co.gameObject);
            Machine.HandleDamage(co.attachedRigidbody.velocity);
		}
    }

}
