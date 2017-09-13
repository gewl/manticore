using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceMouse : EntityComponent {

    Vector3 lastSafeRotation;

    protected override void Subscribe()
    {
        lastSafeRotation = Vector3.zero;
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);
    }
    
    void OnUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100))
		{
			Vector3 hitPoint = hit.point;
            hitPoint.y = 0f;
            Vector3 characterToHitpoint = (hitPoint - transform.position).normalized;

            UpdateBodyRotation(characterToHitpoint);
		}
    }

	void UpdateBodyRotation(Vector3 normalizedMousePosition)
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
		Vector3 tempRotation = transform.rotation.eulerAngles;
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

        transform.rotation = Quaternion.Euler(tempRotation);

		transform.rotation = Quaternion.LookRotation(-normalizedMousePosition);
	}

    float SnapValuesToBreakpoint(float initialValue) {
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
}
