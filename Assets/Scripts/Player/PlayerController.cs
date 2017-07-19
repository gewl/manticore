
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
    private Vector3 lastSafeRotation = new Vector3(0f, 0f, -1.0f);

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

            UpdateBodyRotation(characterToHitpoint);
		}
    }

    private float SnapValuesToBreakpoint(float initialValue) {
		float[] breakpoints = new float[4] { -1f, -.7f, .7f, 1f };

        // 0f is default breakpoint because it's harder to check for
        float closestBreakpoint = 0f;
        float newValue = initialValue;

        for (int i = 0; i < breakpoints.Length; i++)
        {
            float tempRemainder = initialValue - breakpoints[i];

            if (Mathf.Abs(tempRemainder) < Mathf.Abs(newValue)) {
                newValue = tempRemainder;
                closestBreakpoint = breakpoints[i];
            }

        }

        return closestBreakpoint;
	}

	private void UpdateBodyRotation(Vector3 normalMousePosition)
	{
        // "Facing" rotation
        if (Mathf.Abs(normalMousePosition.y) > .01f) {
            normalMousePosition = lastSafeRotation;
        } else {
			normalMousePosition.x = SnapValuesToBreakpoint(normalMousePosition.x);
			normalMousePosition.z = SnapValuesToBreakpoint(normalMousePosition.z);
			if (Mathf.Abs(normalMousePosition.x) == 0.7f)
			{
				normalMousePosition.z = Mathf.Sign(normalMousePosition.z) * 0.7f;
			}
			if (Mathf.Abs(normalMousePosition.z) == 0.7f)
			{
				normalMousePosition.x = Mathf.Sign(normalMousePosition.x) * 0.7f;
			}

            lastSafeRotation = normalMousePosition;
		}

		// "Body" rotation
		Vector3 tempRotation = playerBody.transform.rotation.eulerAngles;
        if (normalMousePosition.x > 0.1f)
	    {
	        tempRotation.z = -30f;
	    } else if (normalMousePosition.x < -0.1f) {
	        tempRotation.z = 30f;
	    } else {
	        tempRotation.z = 0f;
	    }

	    if (normalMousePosition.z > 0.1f) {
	        tempRotation.x = -30f; 
	    } else if (normalMousePosition.z < -0.1f) {
	        tempRotation.x = 30f; 
	    } else {
	        tempRotation.x = 0f; 
	    }

        playerBody.transform.rotation = Quaternion.Euler(tempRotation);


		transform.rotation = Quaternion.LookRotation(-normalMousePosition);
        Debug.Log(transform.rotation);
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
	}

    private void UpdateBodyOrientation() {


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
