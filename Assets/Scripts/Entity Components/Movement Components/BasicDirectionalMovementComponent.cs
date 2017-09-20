using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDirectionalMovementComponent : EntityComponent {

    [SerializeField]
    float baseMoveSpeed;

    float currentMoveSpeed;
    Vector3 currentDirection;

    private void OnEnable()
    {
        entityData.SetSoftAttribute(SoftEntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
		entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
    }

    protected override void Unsubscribe()
    {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
    }

    void OnFixedUpdate()
    {
        Vector3 currentDirection = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentDirection);
		float currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);

		ChangeVelocity(currentDirection, currentMoveSpeed);
	}

    void OnStop()
    {
        ChangeVelocity(Vector3.zero, 0f);
    }

    void ChangeVelocity(Vector3 direction, float moveSpeed)
    {
		direction.Normalize();
        entityData.EntityRigidbody.velocity = direction * moveSpeed;
    }
}
