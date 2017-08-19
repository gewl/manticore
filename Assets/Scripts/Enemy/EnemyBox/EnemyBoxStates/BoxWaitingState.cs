using UnityEngine;

public class BoxWaitingState : EnemyState
{
    int _timer;
    int _nextBulletTimer;
    int _bulletSpeed;
    float rotateStrength = 3f;

    public BoxWaitingState(EnemyStateMachine machine, int timer, int nextBulletTimer, int bulletSpeed)
        : base(machine)
    {
        _timer = timer;
        _nextBulletTimer = nextBulletTimer;
        _bulletSpeed = bulletSpeed;
    }

    public override void Enter()
    {
        Machine.EnemyController.Stop();
    }


    public override void Update()
    {
        _nextBulletTimer--;

        if (_nextBulletTimer == 0)
        {
            Machine.EnemyController.Attack();
            _nextBulletTimer = _bulletSpeed;
        }

        Vector3 positionDifference = Machine.Enemy.transform.position - Machine.Player.transform.position;
        float positionDifferenceMagnitude = positionDifference.magnitude;

        Machine.EnemyController.RotateToFace(Machine.Player, rotateStrength);

        if (positionDifferenceMagnitude < 15f)
        {
            Machine.SwitchState(new BoxCloseState(Machine));
        }

        _timer--;

        if (_timer <= 0)
        {
            Machine.SwitchState(new BoxMiddleState(Machine, _nextBulletTimer));
        }
    }

    public override void HandleTriggerEnter(Collider co)
    {
        base.HandleTriggerEnter(co);
        if (co.gameObject.tag == "Bullet" && co.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Enemy))
        {
            Object.Destroy(co.gameObject);
            Machine.HandleDamage(co.attachedRigidbody.velocity);
        }
    }
}
