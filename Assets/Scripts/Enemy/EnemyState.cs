﻿using UnityEngine;

public abstract class EnemyState
{
    private EnemyStateMachine machine;

    public EnemyStateMachine Machine { get { return machine; } }

    protected int priority = 1;

    public int Priority { get { return priority; } }

    public EnemyState(EnemyStateMachine machine)
    {
        this.machine = machine;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void HandleTriggerEnter(Collider co) { }

}