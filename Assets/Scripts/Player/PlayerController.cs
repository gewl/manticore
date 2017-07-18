
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ref to objs
    private Camera mainCamera;

    // ref to components
	private Rigidbody playerRigidbody;

    // ref to children
    private Transform playerBody;

    // ref to scripts
    private PlayerStateMachine playerMachine;

    // config for functions
	private float speed = 18f;

	void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMachine = ScriptableObject.CreateInstance<PlayerStateMachine>();
        mainCamera = Camera.main;
        playerBody = transform.GetChild(0);
    }

    private void Update()
    {
        playerMachine.Update();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            Vector3 characterToHitpoint = (hitPoint - transform.position).normalized;
            Debug.Log(characterToHitpoint);

            UpdateBodyRotation(characterToHitpoint);
		}
    }

	private void UpdateBodyRotation(Vector3 normalMousePosition)
	{

		Quaternion invertedVelocityRotation = Quaternion.LookRotation(-normalMousePosition);
		transform.rotation = invertedVelocityRotation;

        if (normalMousePosition.x > 0f) {
            
        } else {
            
        }


		//if (playerRigidbody.velocity.x > 0.1f)
		//{
		//	tempRotation.z = -30f;
		//}
		//else if (playerRigidbody.velocity.x < -0.1f)
		//{
		//	tempRotation.z = 30f;
		//}
		//else
		//{
		//	tempRotation.z = 0f;
		//}

		//if (playerRigidbody.velocity.z > 0.1f)
		//{
		//	tempRotation.x = -30f;
		//}
		//else if (playerRigidbody.velocity.z < -0.1f)
		//{
		//	tempRotation.x = 30f;
		//}
		//else
		//{
		//	tempRotation.x = 0f;
		//}

		//playerBody.transform.rotation = Quaternion.Euler(tempRotation);
	}

    void FixedUpdate()
    {
        playerMachine.FixedUpdate();

		Vector3 currentNormalizedVelocity = playerRigidbody.velocity.normalized;
	}

    public void ChangeVelocity(Vector3 direction) {
		direction.Normalize();
        playerRigidbody.velocity = direction * speed;

        UpdateBodyOrientation();
        //UpdateBodyRotation();
	}

    private void UpdateBodyOrientation() {

        float[] breakpoints = new float[4] { -.7f, -.3f, .3f, .7f };

        Quaternion invertedVelocityRotation = Quaternion.LookRotation(-playerRigidbody.velocity);
		transform.rotation = invertedVelocityRotation;
	}

    //private void UpdateBodyRotation() {
    //    Vector3 tempRotation = playerBody.transform.rotation.eulerAngles;

    //    if (playerRigidbody.velocity.x > 0.1f)
    //    {
    //        tempRotation.z = -30f;
    //    } else if (playerRigidbody.velocity.x < -0.1f) {
    //        tempRotation.z = 30f;
    //    } else {
    //        tempRotation.z = 0f;
    //    }

    //    if (playerRigidbody.velocity.z > 0.1f) {
    //        tempRotation.x = -30f; 
    //    } else if (playerRigidbody.velocity.z < -0.1f) {
    //        tempRotation.x = 30f; 
    //    } else {
    //        tempRotation.x = 0f; 
    //    }

    //    playerBody.transform.rotation = Quaternion.Euler(tempRotation);
    //}

    public void Stop() {
        playerRigidbody.velocity = Vector3.zero;
    }
}
