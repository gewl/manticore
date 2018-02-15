using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorStateChangeComponent : EntityComponent {

    Animator animator;
    Rigidbody entityRigidbody;

    float MaxSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }
    float sqrMaxSpeed;

    const string IS_MOVING = "isMoving";
    const string IS_DEAD = "isDead";

    protected override void Awake()
    {
        base.Awake();
        sqrMaxSpeed = MaxSpeed * MaxSpeed;

        animator = GetComponent<Animator>();
        entityRigidbody = GetComponent<Rigidbody>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Move, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);

    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, OnMove);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);

    }

    void OnMove()
    {
        if (!animator.GetBool(IS_MOVING))
        {
            animator.SetBool(IS_MOVING, true);
            entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        }   
    }

    void OnStop()
    {
        if (animator.GetBool(IS_MOVING))
        {
            animator.SetBool(IS_MOVING, false);
            entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
        }   
    }

    void OnDead()
    {
        if (!animator.GetBool(IS_DEAD))
        {
            animator.SetBool(IS_DEAD, true);
        }
    }

    // Only runs while moving.
    void OnFixedUpdate()
    {
        Vector3 currentVelocity = entityRigidbody.velocity;
        currentVelocity.y = 0f;

        float sqrCurrentMoveSpeed = currentVelocity.sqrMagnitude;
        float percentageOfMaxSpeed = sqrCurrentMoveSpeed / sqrMaxSpeed;

        animator.speed = percentageOfMaxSpeed;
    }
}
