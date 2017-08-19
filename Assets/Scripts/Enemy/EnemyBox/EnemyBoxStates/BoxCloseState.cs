using UnityEngine;

public class BoxCloseState : EnemyState
{

    public BoxCloseState(EnemyStateMachine machine)
        : base(machine) { }

    float rotateStrength = 3f;
    Vector3 fleeToPosition;
    bool fleeing = true;

    public override void Enter()
    {
        fleeToPosition = -2f * (Machine.Player.transform.position - Machine.Enemy.transform.position);
        fleeToPosition.y = 1.5f;
        Machine.EnemyController.ChangeVelocity(fleeToPosition);
        Machine.EnemyController.RotateToFace(fleeToPosition, rotateStrength);
    }

    public override void Update()
    {
        Machine.EnemyController.RotateToFace(fleeToPosition, rotateStrength);

        Vector3 positionDifference = Machine.Enemy.transform.position - Machine.Player.transform.position;
        float positionDifferenceMagnitude = positionDifference.magnitude;

        if (positionDifferenceMagnitude >= 25f)
        {
            Machine.SwitchState(new BoxMiddleState(Machine));
        }
    }

    public override void HandleTriggerEnter(Collider co)
    {
        base.HandleTriggerEnter(co);
        if (co.gameObject.tag == "Bullet" && co.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Enemy))
        {
            Object.Destroy(co.gameObject);
            Machine.SwitchState(new DeadBoxState(Machine, co.attachedRigidbody.velocity));
        }
    }
}
