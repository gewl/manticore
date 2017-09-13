using UnityEngine;

public class BlinkingState : PlayerState
{
    public BlinkingState(PlayerStateMachine machine)
        : base(machine) { }

    private int timer;
    private GameObject playerBody;

    public override void Enter()
    {
        base.Enter();
        Machine.BlinkBody.SetActive(true);
        playerBody = Machine.Player.transform.GetChild(0).gameObject;

        timer = 5;
    }

    public override void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            hitPoint.y = Machine.Player.transform.position.y;
            Vector3 characterToHitpoint = (hitPoint - Machine.Player.transform.position);

            if (characterToHitpoint.magnitude >= 10f)
            {
                characterToHitpoint = Vector3.ClampMagnitude(characterToHitpoint, 10f);
                characterToHitpoint.y = Machine.PlayerRigidbody.transform.position.y;
            }
            Machine.BlinkBody.transform.position = Machine.Player.transform.position + characterToHitpoint;

            Machine.PlayerController.UpdateBodyRotation(characterToHitpoint.normalized);
		}
        Machine.BlinkBody.transform.rotation = playerBody.transform.rotation;

        if (timer > 0f && Input.GetKey(KeyCode.LeftShift))
        {
            timer--;
        }
        else if (timer > 0f)
        {
            Machine.SwitchState(new StandardState(Machine));
        }
        else if (timer <= 0f)
        {
            Machine.BlinkBody.SetActive(false);
            Vector3 blinkPosition = Machine.BlinkBody.transform.position;
            blinkPosition.y = 0f;
            Machine.Player.transform.position = blinkPosition;
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
            Machine.PlayerController.ChangeVelocity(direction, .1f);
        }
    }

    public override void Exit()
    {
        base.Exit();

        Machine.BlinkBody.SetActive(false);
    }

    public override void HandleTriggerEnter(Collider co)
    {
        GameObject colliderGo = co.gameObject;
        if (colliderGo.tag == "Bullet" && colliderGo.GetComponent<BulletBehavior>().IsUnfriendly(Machine.Player))
        {
            Debug.Log("Ouch!");
            Object.Destroy(co.gameObject);
            Machine.SwitchState(new DamagedState(Machine, co.attachedRigidbody.velocity));
        }
    }
}