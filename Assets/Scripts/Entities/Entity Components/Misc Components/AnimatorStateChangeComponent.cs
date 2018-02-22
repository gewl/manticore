using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AnimatorStateChangeComponent : EntityComponent {

    Animator animator;
    NavMeshAgent agent;

    float MaxSpeed { get { return entityInformation.Data.BaseMoveSpeed; } }

    const string IS_MOVING = "isMoving";
    const string IS_DEAD = "isDead";
    const string IS_STUNNED = "isStunned";

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.Move, OnMove);
        entityEmitter.SubscribeToEvent(EntityEvents.Stop, OnStop);
        entityEmitter.SubscribeToEvent(EntityEvents.Dead, OnDead);
        entityEmitter.SubscribeToEvent(EntityEvents.Stun, OnStun);
        entityEmitter.SubscribeToEvent(EntityEvents.Unstun, OnUnstun);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, OnMove);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnStun);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnUnstun);
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

    void OnStun()
    {
        if (!animator.GetBool(IS_STUNNED))
        {
            animator.SetBool(IS_STUNNED, true);
        }
    }

    void OnUnstun()
    {
        Debug.Log("unstunning");
        if (animator.GetBool(IS_STUNNED))
        {
            animator.SetBool(IS_STUNNED, false);
        }
    }

    // Only runs while moving.
    void OnFixedUpdate()
    {
        float currentMoveSpeed = agent.speed;
        float percentageOfMaxSpeed = currentMoveSpeed / MaxSpeed;

        animator.speed = percentageOfMaxSpeed;
    }
}
