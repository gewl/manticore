using UnityEngine;

public class AttemptingParryState : PlayerState
{
    public AttemptingParryState(PlayerStateMachine machine)
        : base(machine) { }

    private int timer;

    public override void Enter()
    {
        base.Enter();

        timer = 20;
        Machine.PlayerController.parryBox.SetActive(true);
        Machine.PlayerController.parryBox.GetComponent<ParryHandler>().ParriedBullet += (bullet) => Machine.SwitchState(new SuccessfulParryState(Machine, bullet));
    }

    public override void Update()
    {
        if (timer > 0)
        {
            timer--;
        }
        else
        {
            Machine.SwitchState(new StandardState(Machine));
        }
    }

    public override void FixedUpdate()
    {
        float horizontalKeyValue = Input.GetAxis("HorizontalKey");
        float verticalKeyValue = Input.GetAxis("VerticalKey");

        Vector3 currentVelocity = Machine.PlayerRigidbody.velocity;

        if (Mathf.Abs(horizontalKeyValue) < 0.1f && Mathf.Abs(verticalKeyValue) < 0.1f)
        {
            Machine.PlayerController.Stop();
        }
        else
        {
            Vector3 direction = new Vector3(horizontalKeyValue, 0, verticalKeyValue);
            Machine.PlayerController.ChangeVelocity(direction, 0.7f);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Machine.PlayerController.parryBox.SetActive(false);
    }

    public override void HandleTriggerEnter(Collider co)
    {
        GameObject colliderGo = co.gameObject;
        if (colliderGo.tag == "Bullet" && !colliderGo.GetComponent<BulletBehavior>().IsFriendly(Machine.Player))
        {
            Machine.PlayerController.ChangeVelocity(co.attachedRigidbody.velocity, 0.7f);
            Debug.Log("Ouch!");
            Machine.SwitchState(new DamagedState(Machine));
        }
    }
}