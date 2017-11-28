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

    Bounds entityBounds;

    Vector3 currentVelocity;

    float currentMoveSpeed;
    Vector3 currentDirection;

    private void OnEnable()
    {
        entityBounds = GetComponent<Collider>().bounds;
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
            ChangeVelocity(-Vector3.up, GameManager.GetEntityFallSpeed);
        }
        else
        {
            Vector3 currentDirection = (Vector3)entityData.GetAttribute(EntityAttributes.CurrentDirection);
            float currentMoveSpeed = (float)entityData.GetAttribute(EntityAttributes.CurrentMoveSpeed);

            ChangeVelocity(currentDirection, currentMoveSpeed);
        }

        Vector3 projectedMovement = currentVelocity * Time.deltaTime;

        if (isOnARamp)
        {
            Vector3 testPosition = transform.position + projectedMovement;
            RaycastHit hit;
            if (Physics.Raycast(testPosition, Vector3.down, out hit, terrainMask))
            {
                Vector3 newPosition = hit.point;
                newPosition.y += entityBounds.extents.y;
                transform.position = newPosition;
            }
            else
            { 
                transform.position += projectedMovement;
            }
        }
        else
        {
            transform.position += projectedMovement;
        }
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
        currentVelocity = direction * moveSpeed;
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
        if (collision.gameObject.CompareTag("Ramp"))
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
        if (collision.gameObject.CompareTag("Ramp"))
        {
            isOnARamp = false;
        }
    }
}
