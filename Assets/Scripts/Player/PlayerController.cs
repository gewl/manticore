using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ref to components
	private Rigidbody playerRigidbody;

    // ref to scripts
    private PlayerStateMachine playerMachine;

    // config for functions
	private float speed = 15f;

	void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMachine = ScriptableObject.CreateInstance<PlayerStateMachine>();
    }

    private void Update()
    {
        playerMachine.Update();
    }

    void FixedUpdate()
    {
        playerMachine.FixedUpdate();
        //Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        //ChangeVelocity(direction);
	}

    public void ChangeVelocity(Vector3 direction) {
		direction.Normalize();
		playerRigidbody.velocity = direction * speed;
	}

    public void Stop() {
        playerRigidbody.velocity = Vector3.zero;
    }
}
