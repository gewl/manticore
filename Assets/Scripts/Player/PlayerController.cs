
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ref to components
	private Rigidbody playerRigidbody;

    // ref to children
    private Transform playerBody;

    // ref to scripts
    private PlayerStateMachine playerMachine;

    // config for functions
	private float speed = 18f;

    private Vector3 lastVelocity;

	void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMachine = ScriptableObject.CreateInstance<PlayerStateMachine>();

        playerBody = transform.GetChild(0);
    }

    private void Update()
    {
        playerMachine.Update();
    }

    void FixedUpdate()
    {
        playerMachine.FixedUpdate();


		Vector3 currentNormalizedVelocity = playerRigidbody.velocity.normalized;
		Debug.Log(currentNormalizedVelocity);

        if (currentNormalizedVelocity == Vector3.zero) {
		}

	}

    public void ChangeVelocity(Vector3 direction) {
		direction.Normalize();
        lastVelocity = direction * speed;
        playerRigidbody.velocity = lastVelocity;

		Quaternion invertedVelocityRotation = Quaternion.LookRotation(-lastVelocity);
		transform.rotation = invertedVelocityRotation;
	}

    public void Stop() {
        playerRigidbody.velocity = Vector3.zero;
    }
}
