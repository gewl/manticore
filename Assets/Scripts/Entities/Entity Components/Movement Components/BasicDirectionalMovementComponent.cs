using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDirectionalMovementComponent : EntityComponent {

    float baseMoveSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }

    LayerMask terrainMask;
    LayerMask allButTerrainMask;
    float distanceToGround;
    int rampCount = 0;
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
        allButTerrainMask = 1 << terrainMask;
        distanceToGround = entityBounds.extents.y;
        entityInformation.SetAttribute(EntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityInformation.SetAttribute(EntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, StopMovement);
		entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
    }

    protected override void Unsubscribe()
    {
		entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, StopMovement);
		entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
    }

    void OnFixedUpdate()
    {
        if (rampCount == 0 && groundedCount == 0)
        {
            ChangeVelocity(-Vector3.up, GameManager.GetEntityFallSpeed);
        }
        else
        {
            Vector3 currentDirection = (Vector3)entityInformation.GetAttribute(EntityAttributes.CurrentDirection);
            float currentMoveSpeed = entityStats.GetMoveSpeed();

            ChangeVelocity(currentDirection, currentMoveSpeed);
        }

        Vector3 projectedMovement = currentVelocity * Time.deltaTime;

        if (rampCount != 0 || groundedCount != 0)
        {
            Vector3 testPosition = transform.position + projectedMovement;
            testPosition.y += entityBounds.extents.y;
            Ray checkTestPosition = new Ray(testPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(checkTestPosition, out hit, entityBounds.size.y * 2f, allButTerrainMask, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point;
                return;
            }
        }

        transform.position += projectedMovement;
    }

    void OnStop()
    {
        if (groundedCount > 0)
        {
            StopMovement();
        }
    }

    void StopMovement()
    {
        ChangeVelocity(Vector3.zero, 0f);
        entityInformation.SetAttribute(EntityAttributes.CurrentDirection, Vector3.zero);
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
            rampCount++;
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
            rampCount--;
        }
    }
}
