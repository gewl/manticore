using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceMouse : EntityComponent {

    Vector3 lastSafeRotation;
    bool rotationFrozen = false;
    Camera mainCamera;
    float cameraRotation;
    Plane playerPlane;

    protected override void Subscribe()
    {
        playerPlane = new Plane(transform.position, Vector3.up);
        mainCamera = Camera.main;
        cameraRotation = mainCamera.transform.rotation.eulerAngles.y;
        lastSafeRotation = Vector3.zero;
        entityEmitter.SubscribeToEvent(EntityEvents.Update, OnUpdate);

		entityEmitter.SubscribeToEvent(EntityEvents.FreezeRotation, OnFreezeRotation);
		entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnFreezeRotation);
		entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnResumeRotation);
		entityEmitter.SubscribeToEvent(EntityEvents.ResumeRotation, OnResumeRotation);
	}

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Update, OnUpdate);

		entityEmitter.UnsubscribeFromEvent(EntityEvents.FreezeRotation, OnFreezeRotation);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnFreezeRotation);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnResumeRotation);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.ResumeRotation, OnResumeRotation);
	}

	void OnFreezeRotation()
	{
		rotationFrozen = true;
	}

	void OnResumeRotation()
	{
		rotationFrozen = false;
	}

	void OnUpdate()
    {
        if (rotationFrozen)
        {
            return;
        }

        Vector3 relativeMousePosition = GameManager.GetMousePositionOnPlayerPlane();

        transform.LookAt(relativeMousePosition);
    }

    void UpdateBodyRotation(Vector2 mousePosition)
    {
        Vector3 newPosition = Quaternion.Euler(0f, cameraRotation, 0f) * new Vector3(mousePosition.x, GameManager.GetPlayerPosition().y, mousePosition.y);
        Debug.DrawLine(transform.position, newPosition, Color.green, 0.1f);

        transform.LookAt(newPosition);
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
