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
    bool isGrounded;

    float currentMoveSpeed;
    Vector3 currentDirection;

    private void OnEnable()
    {
        terrainMask = LayerMask.GetMask("Terrain");
        distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.05f;
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
        if (!isOnARamp && !isGrounded)
        {
            entityData.EntityRigidbody.velocity = -Vector3.up * GameManager.GetEntityFallSpeed;
            return;
        }
        Vector3 currentDirection = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentDirection);
		float currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);

		ChangeVelocity(currentDirection, currentMoveSpeed);
	}

    void OnStop()
    {
        if (isGrounded)
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            isGrounded = false;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = false;
        }
    }
}
