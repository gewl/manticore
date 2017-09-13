using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDirectionalMovementComponent : EntityComponent {

    [SerializeField]
    float baseMoveSpeed;

    float currentMoveSpeed;

    private void OnEnable()
    {
        entityData.SetSoftAttribute(SoftEntityAttributes.BaseMoveSpeed, baseMoveSpeed);
        entityData.SetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed, baseMoveSpeed);
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.DirectionChanged, OnDirectionChanged);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.DirectionChanged, OnDirectionChanged);
    }

    void OnDirectionChanged()
    {
        Vector3 newDirection = (Vector3)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentDirection);
        float currentMoveSpeed = (float)entityData.GetSoftAttribute(SoftEntityAttributes.CurrentMoveSpeed);
    }

    void ChangeVelocity(Vector3 direction, float moveSpeed)
    {
		direction.Normalize();
        entityData.EntityRigidbody.velocity = direction * moveSpeed;
    }
}
