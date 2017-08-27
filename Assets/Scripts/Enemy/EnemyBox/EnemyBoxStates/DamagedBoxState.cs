using System.Collections.Generic;
using UnityEngine;

public class DamagedBoxState : EnemyState
{
    Vector3 bulletVelocity;
    Material originalSkin;

    public DamagedBoxState(EnemyStateMachine machine, Vector3 colliderVelocity)
        : base(machine)
    {
        bulletVelocity = colliderVelocity;
    }

    private int damagedTimer = 31;
    private int originalTimer;

    private GameObject body;

    public override void Enter()
    {
        base.Enter();
        body = Machine.Enemy.transform.GetChild(0).gameObject;
        Machine.EnemyController.ChangeVelocity(bulletVelocity, 0.5f);

        Machine.EnemyRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Machine.EnemyRigidbody.AddTorque(0f, Mathf.Sqrt(Mathf.Abs(bulletVelocity.x * bulletVelocity.z)), 0f);

        originalSkin = Machine.EnemyController.MeshRenderer.material;
        originalTimer = damagedTimer;
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

        float skinTransitionComplete = (originalTimer - damagedTimer) / originalTimer;
        skinTransitionComplete = Mathf.Sqrt(1 - skinTransitionComplete);
        Machine.EnemyController.MeshRenderer.material.Lerp(originalSkin, Machine.EnemyController.DamagedSkin, skinTransitionComplete);

        if (damagedTimer <= 0)
        {
            Machine.EnemyController.Stop();
            Machine.SwitchState(new BoxMiddleState(Machine));
        }
        damagedTimer--;
    }

    public override void Exit()
    {
        Machine.EnemyRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
}