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
    const string FIRE_PRIMARY = "firePrimary";
    const string PARRY = "parry";

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
        entityEmitter.SubscribeToEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.SubscribeToEvent(EntityEvents.ParrySwing, OnParrySwing);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Move, OnMove);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stop, OnStop);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Dead, OnDead);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Stun, OnStun);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.Unstun, OnUnstun);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.PrimaryFire, OnPrimaryFire);
        entityEmitter.UnsubscribeFromEvent(EntityEvents.ParrySwing, OnParrySwing);
    }

    void OnMove()
    {
        if (AnimatorContainsParameter(IS_MOVING) && !animator.GetBool(IS_MOVING))
        {
            animator.SetBool(IS_MOVING, true);

            if (agent != null)
            {
                entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
            }
        }   
    }

    void OnStop()
    {
        if (AnimatorContainsParameter(IS_MOVING) && animator.GetBool(IS_MOVING))
        {
            animator.SetBool(IS_MOVING, false);

            if (agent != null)
            {
                entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
            }
        }   
    }

    void OnDead()
    {
        if (AnimatorContainsParameter(IS_DEAD) && !animator.GetBool(IS_DEAD))
        {
            animator.SetBool(IS_DEAD, true);
        }
    }

    void OnStun()
    {
        if (AnimatorContainsParameter(IS_STUNNED) && !animator.GetBool(IS_STUNNED))
        {
            animator.SetBool(IS_STUNNED, true);
        }
    }

    void OnUnstun()
    {
        if (AnimatorContainsParameter(IS_STUNNED) && animator.GetBool(IS_STUNNED))
        {
            animator.SetBool(IS_STUNNED, false);
        }
    }

    void OnPrimaryFire()
    {
        if (AnimatorContainsParameter(FIRE_PRIMARY))
        {
            animator.SetTrigger(FIRE_PRIMARY);
        }

    }

    void OnParrySwing()
    {
        if (AnimatorContainsParameter(PARRY))
        {
            animator.SetTrigger(PARRY);
        }
    }

    // Only runs while moving.
    void OnFixedUpdate()
    {
        float currentMoveSpeed = agent.speed;
        float percentageOfMaxSpeed = currentMoveSpeed / MaxSpeed;

        animator.speed = percentageOfMaxSpeed;
    }

    bool AnimatorContainsParameter(string name)
    {
        for (int i = 0; i < animator.parameters.Length; i++)
        {
            if (animator.parameters[i].name == name)
            {
                return true;
            }
        }

        return false;
    }
}
