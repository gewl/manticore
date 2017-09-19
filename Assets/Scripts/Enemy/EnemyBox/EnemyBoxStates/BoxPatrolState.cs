using UnityEngine;

public class BoxPatrolState : EnemyState
{

    public BoxPatrolState(EnemyStateMachine machine)
        : base(machine) { }

    // Initialized to -1 because updater, called on entry, increments before retrieving
    int currentPatrolIndex = -1;

    Transform nextPatrolPoint;
    int waitTimer = 0;

    public override void Enter()
    {
        nextPatrolPoint = UpdatePatrolPoint();
    }

    public override void Update()
    {
        float playerDistance = (Machine.Enemy.transform.position - Machine.Player.transform.position).magnitude;
        if (playerDistance < 25f)
        {
            Machine.EnemyController.RotateToFace(Machine.Player, 8f);
            Machine.SwitchState(new BoxMiddleState(Machine));
        }
        if (waitTimer > 0)
        {
            waitTimer--;
            return;
        }
        Machine.EnemyController.RotateToFace(nextPatrolPoint.position);

        Vector3 positionDifference = Machine.Enemy.transform.position - nextPatrolPoint.position;
        float positionDifferenceMagnitude = positionDifference.magnitude;
        Machine.EnemyController.ChangeVelocity(nextPatrolPoint.position - Machine.Enemy.transform.position, .6f);

        if (positionDifferenceMagnitude <= 0.3f)
        {
            nextPatrolPoint = UpdatePatrolPoint();
            waitTimer = 40;
            Machine.EnemyController.Stop();
        }
    }

    public override void HandleTriggerEnter(Collider co)
    {
        base.HandleTriggerEnter(co);
        if (co.gameObject.tag == "Bullet" && co.gameObject.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Enemy))
        {
            Object.Destroy(co.gameObject);
            //Machine.SwitchState(new DeadBoxState(Machine, co.attachedRigidbody.velocity));
            Machine.HandleDamage(co.attachedRigidbody.velocity);
        }
    }

    public void NoticePlayer()
    {
        Debug.Log("player noticed");
    }

    Transform UpdatePatrolPoint()
    {
        currentPatrolIndex++;
        if (currentPatrolIndex > Machine.EnemyController.PatrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return Machine.EnemyController.PatrolPoints[currentPatrolIndex];
    }
}
