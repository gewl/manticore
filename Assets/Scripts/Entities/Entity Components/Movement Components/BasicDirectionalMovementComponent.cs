using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDirectionalMovementComponent : EntityComponent {

    [SerializeField]
    float baseMoveSpeed;

    LayerMask terrainMask;
    float distanceToGround;
    bool isOnARamp;
    int groundedCount = 0;

    float currentMoveSpeed;
    Vector3 currentDirection;

    private void OnEnable()
    {
        terrainMask = LayerMask.NameToLayer("Terrain");
        distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.05f;
        entityData.SetAttribute(EntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityData.SetAttribute(EntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
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
        if (!isOnARamp && groundedCount == 0)
        {
            entityData.EntityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }
        Vector3 currentDirection = (Vector3)entityData.GetAttribute(EntityAttributes.CurrentDirection);
		float currentMoveSpeed = (float)entityData.GetAttribute(EntityAttributes.CurrentMoveSpeed);

		ChangeVelocity(currentDirection, currentMoveSpeed);
	}

    void OnStop()
    {
        if (groundedCount > 0)
        {
            ChangeVelocity(Vector3.zero, 0f);
        }
    }

    void ChangeVelocity(Vector3 direction, float moveSpeed)
    {
		direction.Normalize();
        entityData.EntityRigidbody.velocity = direction * moveSpeed;
    }

    bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == terrainMask)
        {
            groundedCount++;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == terrainMask)
        {
            groundedCount--;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = false;
        }
    }
}
