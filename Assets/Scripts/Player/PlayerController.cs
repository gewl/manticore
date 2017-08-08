﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ref to objs
    public GameObject parryBox;

    // ref to components
	private Rigidbody playerRigidbody;

    // ref to children
    private Transform playerBody;

    // ref to scripts
    private PlayerStateMachine playerMachine;

    // config for functions
	private float speed = 18f;
    private Vector3 lastSafeRotation = new Vector3(0f, 0f, -1.0f);

    // cooldown on actions
    private int parryCooldown;

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
	}

    private void OnTriggerEnter(Collider other)
    {
        playerMachine.HandleTriggerEnter(other);
    }

    #region movement
    // used for limiting/handling rotation
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

	public void UpdateBodyRotation(Vector3 normalizedMousePosition)
	{
        // "Facing" rotation
        if (Mathf.Abs(normalizedMousePosition.y) > .01f) 
        {
            normalizedMousePosition = lastSafeRotation;
        } 
        else 
        {
			normalizedMousePosition.x = SnapValuesToBreakpoint(normalizedMousePosition.x);
			normalizedMousePosition.z = SnapValuesToBreakpoint(normalizedMousePosition.z);
			if (Mathf.Abs(normalizedMousePosition.x) == 0.7f)
			{
				normalizedMousePosition.z = Mathf.Sign(normalizedMousePosition.z) * 0.7f;
			}
			if (Mathf.Abs(normalizedMousePosition.z) == 0.7f)
			{
				normalizedMousePosition.x = Mathf.Sign(normalizedMousePosition.x) * 0.7f;
			}

            lastSafeRotation = normalizedMousePosition;
		}

		// "Body" rotation
		Vector3 tempRotation = playerBody.transform.rotation.eulerAngles;
        if (normalizedMousePosition.x > 0.1f)
	    {
	        tempRotation.z = -30f;
	    } 
        else if (normalizedMousePosition.x < -0.1f) 
        {
	        tempRotation.z = 30f;
	    } 
        else 
        {
	        tempRotation.z = 0f;
	    }

	    if (normalizedMousePosition.z > 0.1f) 
        {
	        tempRotation.x = -30f; 
	    } 
        else if (normalizedMousePosition.z < -0.1f)
        {
	        tempRotation.x = 30f; 
	    } 
        else 
        {
	        tempRotation.x = 0f; 
	    }

        playerBody.transform.rotation = Quaternion.Euler(tempRotation);

		transform.rotation = Quaternion.LookRotation(-normalizedMousePosition);
	}

    public void ChangeVelocity(Vector3 direction, float movementModifier = 1f) 
    {
		direction.Normalize();
        playerRigidbody.velocity = direction * speed * movementModifier;
	}

    public void Stop() 
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }
    #endregion

}
