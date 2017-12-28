using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDirectionalMovementComponent : EntityComponent {

    float baseMoveSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }

    LayerMask terrainMask;
    float distanceToGround;
    bool isOnARamp;
    int groundedCount = 0;

    Bounds entityBounds;

    Vector3 currentVelocity;

    float currentMoveSpeed;
    Vector3 currentDirection;

    protected override void OnEnable()
    {
        base.OnEnable();
        entityBounds = GetComponent<Collider>().bounds;
        terrainMask = LayerMask.NameToLayer("Terrain");
        distanceToGround = GetComponent<Collider>().bounds.extents.y + 0.05f;
        entityInformation.SetAttribute(EntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
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
            Vector3 currentDirection = (Vector3)entityInformation.GetAttribute(EntityAttributes.CurrentDirection);
            float currentMoveSpeed = (float)entityInformation.GetAttribute(EntityAttributes.CurrentMoveSpeed);

            ChangeVelocity(currentDirection, currentMoveSpeed);
        }

        Vector3 projectedMovement = currentVelocity * Time.deltaTime;

        if (isOnARamp)
        {
            Vector3 testPosition = transform.position + projectedMovement;
            Ray checkTestPosition = new Ray(testPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(checkTestPosition, out hit, entityBounds.size.y, terrainMask))
            {
                Vector3 newPosition = hit.point;
                newPosition.y += entityBounds.extents.y;
                transform.position = newPosition;
                return;
            }
        }

        transform.position += projectedMovement;
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
